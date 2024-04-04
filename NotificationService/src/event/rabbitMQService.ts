import * as amqp from "amqplib/callback_api";

class RabbitMQService {
  private connectionUrl: string;
  private connection: amqp.Connection | null;
  private retryDelay: number;
  private maxRetryAttempts: number;
  private currentRetryAttempts: number;

  constructor(
    connectionUrl: string,
    retryDelay: number = 5000,
    maxRetryAttempts: number = 10
  ) {
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
              console.error(
                `Failed to connect to RabbitMQ. Retrying in ${
                  this.retryDelay / 1000
                } seconds...`
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
            resolve();
          }
        });
      };

      connectWithRetry();
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