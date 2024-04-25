import * as amqp from "amqplib/callback_api";
import { v4 as uuidv4 } from 'uuid';

class RabbitMQService {
	private connectionUrl: string;
	private connection: amqp.Connection | null;
	private channelRPC: amqp.Channel | null;
	private retryDelay: number;
	private maxRetryAttempts: number;
	private currentRetryAttempts: number = 0;
	private rpcQueueName: string | null = null;
  
	constructor(
	  connectionUrl: string,
	  retryDelay: number = 5000,
	  maxRetryAttempts: number = 10
	) {
	  this.connectionUrl = connectionUrl;
	  this.connection = null;
	  this.channelRPC = null;
	  this.rpcQueueName = null;
	  this.retryDelay = retryDelay;
	  this.maxRetryAttempts = maxRetryAttempts;
	}
  
	async connect(): Promise<void> {
	  if (this.connection) {
		if (!this.channelRPC) {
		  await this.setupChannelRPC();
		}
		return;
	  }
  
	  return new Promise<void>((resolve, reject) => {
		const connectWithRetry = () => {
		  amqp.connect(this.connectionUrl, async (error, connection) => {
			if (error) {
			  if (this.currentRetryAttempts < this.maxRetryAttempts) {
				this.currentRetryAttempts++;
				console.error(
				  `Failed to connect to RabbitMQ. Retrying in ${this.retryDelay / 1000} seconds...`
				);
				setTimeout(connectWithRetry, this.retryDelay);
			  } else {
				reject(
				  new Error(
					"Exceeded maximum retry attempts. Could not connect to RabbitMQ."
				  )
				);
			  }
			} else {
			  this.connection = connection;
			  this.currentRetryAttempts = 0;
  
			  try {
				await this.setupChannelRPC(); // Set up the channel and queue on successful connection
				resolve();
			  } catch (err) {
				reject(err); // Handle setup errors
			  }
			}
		  });
		};
  
		connectWithRetry(); // Start the connection with retry logic
	  });
	}
  
	private setupChannelRPC(): Promise<void> {
		return new Promise<void>((resolve, reject) => {
		  if (!this.connection) {
			reject(new Error("Connection not established."));
			return;
		  }
	
		  this.connection.createChannel((error, channel) => {
			if (error) {
			  reject(error); // Error while creating the channel
			} else {
			  this.channelRPC = channel; // Set the channelRPC
			  
			  // Set up the RPC queue (exclusive and autoDelete to ensure it's only used for this session)
			  this.channelRPC.assertQueue(
				"",
				{
				  exclusive: true,
				},
				(err, result) => {
				  if (err) {
					reject(err); // Error while asserting the queue
				  } else {
					this.rpcQueueName = result.queue; // Store the RPC queue name for later use
					resolve(); // Successfully set up the queue
				  }
				}
			  );
			}
		  });
		});
	  }

  async consumeEvents(
    exchange: string,
    callback: (eventData: any) => void
  ): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      if (!this.connection) {
        reject(new Error("RabbitMQ connection is not established."));
        return;
      }

      this.connection.createChannel((error, channel) => {
        if (error) {
          reject(error);
          return;
        }

        channel.assertExchange(exchange, "fanout", { durable: false });
        channel.assertQueue("", { exclusive: true }, (error, q) => {
          if (error) {
            reject(error);
            return;
          }

          channel.bindQueue(q.queue, exchange, "");

          channel.consume(
            q.queue,
            (message) => {
              if (message) {
                const eventData = JSON.parse(message.content.toString());
                callback(eventData);
              }
            },
            { noAck: true }
          );

          console.log(`Listening for ${exchange} events on queue: ${q.queue}`);
          resolve();
        });
      });
    });
  }

   async consumeRPCEvents(
    queueName: string,
    messageBody: string,
    callback: (eventData: any) => void
  ): Promise<void> {
    if (!this.connection || !this.channelRPC || !this.rpcQueueName) {
      throw new Error("RabbitMQ connection or channel is not properly initialized.");
    }

    const correlationId = uuidv4(); // Generate a unique correlation ID

    // Consume messages from the existing RPC queue
    this.channelRPC.consume(
      this.rpcQueueName,
      (msg) => {
        if (msg && msg.properties.correlationId === correlationId) {
          const eventData = JSON.parse(msg.content.toString());
          callback(eventData);
		  if (typeof msg.fields.consumerTag === "string") {
			this.channelRPC?.cancel(msg.fields.consumerTag);
		  }
        }
      },
      { noAck: true }
    );

    // Send the message body to the specified queue, with the defined reply-to queue
    this.channelRPC.sendToQueue(queueName, Buffer.from(messageBody), {
      correlationId: correlationId,
      replyTo: this.rpcQueueName, // Use the predefined reply-to queue
    });
  }

  async closeConnection(): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      if (this.connection) {
        this.connection.close((error) => {
          if (error) {
            reject(error);
          } else {
            resolve();
          }
        });
      } else {
        resolve();
      }
    });
  }
}

export default RabbitMQService;
