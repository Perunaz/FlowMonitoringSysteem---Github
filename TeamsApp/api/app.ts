import express from "express";
import cors from "cors";
import morgan from "morgan";
import helmet from "helmet";
import dotenv from 'dotenv';
import { createServer } from 'http';
import WebSocket from 'ws';
import { validation, cast, entityNotFound } from "./errors";
import crypto from "crypto";

import RabbitMQService from "./rabbitMQService";
import FlowController from "./src/controllers/flowController";
import FlowRouter from "./src/routes/flow.routes";
import TokenRouter from "./src/routes/token.routes";
import TokenController from "./src/controllers/tokenController";

import { PublicClientApplication, AuthorizationUrlRequest } from '@azure/msal-node';


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
const rabbitMQService = new RabbitMQService(process.env.RABBITMQ_URL || "amqp://rabbitmq:5672");
const flowController = new FlowController(rabbitMQService, wss);
const tokenController = new TokenController(rabbitMQService);

const flowRouter = new FlowRouter(flowController);
const tokenRouter = new TokenRouter(tokenController);

app.use("/api/v1/flow", flowRouter.getRouter());
app.use("/api/v1/token", tokenRouter.getRouter());

// error handling middleware
app.use(validation);
app.use(cast);
app.use(entityNotFound);

const msalConfig = {
  auth: {
    clientId: process.env.REACT_APP_API_CLIENT_ID || "",
    authority: `https://login.microsoftonline.com/${process.env.REACT_APP_API_TENANT_ID}`,
    redirectUri: "http://localhost:7071/redirect", // Adjust your redirect URI as needed
  }
};

function generatePKCE() {
  const codeVerifier = crypto.randomBytes(32).toString('hex'); // Generate a random code verifier
  const codeChallenge = crypto.createHash('sha256').update(codeVerifier).digest('base64url'); // Generate code challenge

  return { codeVerifier, codeChallenge };
}

const msalInstance = new PublicClientApplication(msalConfig);
// Route to initiate login
app.get('/api/v1/login', async (req, res) => {
  const { codeVerifier, codeChallenge } = generatePKCE();
  // Define the authorization URL request, including the redirect URI
  const authCodeUrlRequest: AuthorizationUrlRequest = {
    scopes: ['https://service.flow.microsoft.com//.default', 'offline_access'], // Add desired scopes
    redirectUri: msalConfig.auth.redirectUri,
    codeChallenge,
    codeChallengeMethod: 'S256',  // Required parameter
  };

  try {
    // Generate the authorization URL
    const authUrl = await msalInstance.getAuthCodeUrl(authCodeUrlRequest);
    console.log(authUrl);
    // Redirect the user to the authorization URL
    res.redirect(authUrl);
  } catch (error) {
    console.error('Error creating authorization URL:', error);
    res.status(500).send('Error initiating login process');
  }
});

// Start the HTTP server
// const PORT = process.env.WS_PORT || 8080;
// server.listen(PORT, () => {
//   console.log(`WebSocket server listening on port ${PORT}`);
// });

flowController
  .listenForFlowStatusEvents()
  .then(() => {
    console.log("Listening for delivery events...");
  })
  .catch((error) => {
    console.error("Error while listening for delivery events:", error);
  });

export default app;