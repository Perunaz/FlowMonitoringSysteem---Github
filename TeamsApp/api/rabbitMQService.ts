import * as amqp from "amqplib/callback_api";


class RabbitMQService {
	private connectionUrl: string;
	private connection: amqp.Connection | null;
	private retryDelay: number;
	private maxRetryAttempts: number;
	private currentRetryAttempts: number;
  
	constructor(connectionUrl: string, retryDelay: number = 5000, maxRetryAttempts: number = 10) {
	  this.connectionUrl = connectionUrl;
	  this.connection = null;
	  this.retryDelay = retryDelay;
	  this.maxRetryAttempts = maxRetryAttempts;
	  this.currentRetryAttempts = 0;
	}
  
	async connect(): Promise<void> {
	  return new Promise<void>((resolve, reject) => {
		const connectWithRetry = () => {
		  amqp.connect(this.connectionUrl, (error, connection) => {
			if (error) {
			  if (this.currentRetryAttempts < this.maxRetryAttempts) {
				this.currentRetryAttempts++;
				console.error(`Failed to connect to RabbitMQ. Retrying in ${this.retryDelay / 1000} seconds...`);
				setTimeout(connectWithRetry, this.retryDelay);
			  } else {
				reject(new Error("Exceeded maximum retry attempts. Could not connect to RabbitMQ."));
			  }
			} else {
			  this.connection = connection;
			  this.currentRetryAttempts = 0;
			  resolve();
			}
		  });
		};
  
		connectWithRetry();
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
			channel.assertQueue('', { exclusive: true }, (error, q) => {
			  if (error) {
				reject(error);
				return;
			  }
	  
			  channel.bindQueue(q.queue, exchange, '');
	  
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