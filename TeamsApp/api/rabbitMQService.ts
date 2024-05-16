import * as amqp from "amqplib/callback_api";
import { v4 as uuidv4 } from 'uuid';

class RabbitMQService {
	private connectionUrl: string;
	private connection: amqp.Connection | null;
	private channelReceivingRPC: amqp.Channel | null;
	private channelProducingTokenRPC: amqp.Channel | null;
	private retryDelay: number;
	private maxRetryAttempts: number;
	private currentRetryAttempts: number = 0;
	private rpcQueueName: string | null;
	private accessToken: string | null;

	constructor(
		connectionUrl: string,
		retryDelay: number = 5000,
		maxRetryAttempts: number = 10
	) {
		this.connectionUrl = connectionUrl;
		this.connection = null;
		this.channelReceivingRPC = null;
		this.channelProducingTokenRPC = null;
		this.rpcQueueName = null;
		this.accessToken = null;
		this.retryDelay = retryDelay;
		this.maxRetryAttempts = maxRetryAttempts;
	}

	async connect(): Promise<void> {
		if (this.connection) {
			if (!this.channelReceivingRPC) {
				await this.setupChannelReceivingRPC();
			}

			if (!this.channelProducingTokenRPC) {
				//await this.setupChannelProducingRPC("token");
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
							await this.setupChannelReceivingRPC();
							//await this.setupChannelProducingRPC("token"); // Set up the channel and queue on successful connection
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

	private setupChannelReceivingRPC(): Promise<void> {
		return new Promise<void>((resolve, reject) => {
			if (!this.connection) {
				reject(new Error("Connection not established."));
				return;
			}

			this.connection.createChannel((error, channel) => {
				if (error) {
					reject(error); // Error while creating the channel
				} else {
					this.channelReceivingRPC = channel; // Set the channelReceivingRPC

					// Set up the RPC queue (exclusive and autoDelete to ensure it's only used for this session)
					this.channelReceivingRPC.assertQueue(
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

	private setupChannelProducingRPC(queueName: string): Promise<void> {
		return new Promise<void>((resolve, reject) => {
			if (!this.connection) {
				reject(new Error("Connection not established."));
				return;
			}
	
			this.connection.createChannel((error1, channel) => {
				if (error1) {
					reject(error1); // Error while creating the channel
				} else {
					this.channelProducingTokenRPC = channel;
	
					const queue = queueName;
	
					channel.assertQueue(queue, {
						durable: false
					});
					channel.prefetch(1);
					channel.consume(queue, (msg) => {
						if (msg !== null) {
							const response = this.getAccessToken();
							if (response !== null) {
								channel.sendToQueue(msg.properties.replyTo,
									Buffer.from(response), {
										correlationId: msg.properties.correlationId
									});
								channel.ack(msg);
							} else {
								// If response is null, handle it here
								console.error("Access token is null");
							}
						}
					});
	
					resolve();
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
		if (!this.connection || !this.channelReceivingRPC || !this.rpcQueueName) {
			throw new Error("RabbitMQ connection or channel is not properly initialized.");
		}

		const correlationId = uuidv4(); // Generate a unique correlation ID

		// Consume messages from the existing RPC queue
		this.channelReceivingRPC.consume(
			this.rpcQueueName,
			(msg) => {
				if (msg && msg.properties.correlationId === correlationId) {
					try {
						const eventData = JSON.parse(msg.content.toString());
						callback(eventData);
					} catch (error) {
						console.error("Failed to parse message content:", error);
						// Log the problematic message content for debugging
						console.error("Raw message content:", msg.content.toString());
						// Handle the error (e.g., requeue the message or send to a dead-letter queue)
					}
					if (typeof msg.fields.consumerTag === "string") {
						this.channelReceivingRPC?.cancel(msg.fields.consumerTag);
					}
				}
			},
			{ noAck: true }
		);

		// Send the message body to the specified queue, with the defined reply-to queue
		this.channelReceivingRPC.sendToQueue(queueName, Buffer.from(messageBody), {
			correlationId: correlationId,
			replyTo: this.rpcQueueName, // Use the predefined reply-to queue
		});
	}

	async sendTokens(
		accessToken: string
	): Promise<void> {
		return new Promise<void>((resolve, reject) => {
			if (!this.connection) {
				reject(new Error("RabbitMQ connection is not established."));
				return;
			}

			// Create a channel
			this.connection.createChannel(function (error1, channel) {
				if (error1) {
					reject(error1);
					return;
				}

				// Ensure the queue exists
				channel.assertQueue("tokens", {
					durable: false,
				});

				// Prepare the token message
				const messageString = JSON.stringify(accessToken);

				// Send the message to the queue
				channel.sendToQueue("tokens", Buffer.from(messageString));
			});
		});
	};

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

	isConnected(): boolean {
		if (this.connection) {
			return true;
		} else {
			return false;
		}
	}

	async restartConnection(): Promise<void> {
		this.connection = null;
		this.channelReceivingRPC = null;
		this.channelProducingTokenRPC = null;
		this.rpcQueueName = null;
		return await this.connect();
	};

	setAccessToken(accessToken: string) {
		this.accessToken = accessToken;
	}

	getAccessToken(): string | null {
		return this.accessToken;
	}
}

export default RabbitMQService;
