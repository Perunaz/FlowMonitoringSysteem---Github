import express, { Application, NextFunction, Request, Response } from "express";
import cors from "cors";
import helmet from "helmet";

import { validation, cast, entityNotFound } from "./errors";
import RabbitMQService from "./src/event/rabbitMQService";

const app: Application = express();

app.use(helmet());
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(express.json());


// Create an instance of the RabbitMQService with the connection URL and exchange
const rabbitMQService = new RabbitMQService("amqp://rabbitmq");

//const emailSender = new EmailSender(rabbitMQService);

// emailSender
//   .listenForFlowStatusChanges()
//   .then(() => {
//     console.log("Listening for flow events...");
//   })
//   .catch((error) => {
//     console.error("Error while listening for flow events:", error);
//   });

app.use("*", (req: Request, res: Response) => {
    res.status(404).json({
      message: "Not found",
    });
  });

// error handling middleware
app.use(validation);
app.use(cast);
app.use(entityNotFound);

app.use((err: any, req: Request, res: Response, next: NextFunction) => {
    console.error(`${err.name}: ${err.message}`);
    res.status(500).json({
      error: {
        message: "An unexpected error occurred while processing your request.",
      },
    });
  });
  
export default app;