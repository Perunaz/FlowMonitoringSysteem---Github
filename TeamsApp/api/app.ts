import express from "express";
import cors from "cors";
import morgan from "morgan";
import helmet from "helmet";
import dotenv from 'dotenv';
import { createServer } from 'http';
import WebSocket from 'ws';

import RabbitMQService from "./rabbitMQService";
import FlowController from "./src/controllers/flowController";
import FlowRouter from "./src/routes/flow.routes";

dotenv.config();
const app = express();

app.use(helmet());
app.use(cors());
app.use(morgan("dev"));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(express.json());

// Create HTTP server and WebSocket server
const server = createServer(app);
const wss = new WebSocket.Server({ server });


// Create an instance of the RabbitMQService with the connection URL and exchange
const rabbitMQService = new RabbitMQService(process.env.RABBITMQ_URL || "amqp://rabbitmq");
const flowController = new FlowController(rabbitMQService, wss);

const flowRouter = new FlowRouter(flowController);

app.use("/api/v1/flow", flowRouter.getRouter());


// Start the HTTP server
const PORT = process.env.WS_PORT || 8080;
server.listen(PORT, () => {
  console.log(`WebSocket server listening on port ${PORT}`);
});

flowController
  .listenForFlowStatusEvents()
  .then(() => {
    console.log("Listening for delivery events...");
  })
  .catch((error) => {
    console.error("Error while listening for delivery events:", error);
  });

export default app;