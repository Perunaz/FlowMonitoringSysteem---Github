import RabbitMQService from "../../rabbitMQService";
import WebSocket from 'ws';
import { Request, Response } from "express";

class FlowController {
    private rabbitMQService: RabbitMQService;
    private wss: WebSocket.Server;

    constructor(rabbitMQService: RabbitMQService, wss: WebSocket.Server) {
        this.rabbitMQService = rabbitMQService;
        this.wss = wss;

        this.setupWebSocket();
    }

    setupWebSocket() {
        this.wss.on('connection', () => {
            console.log('New WebSocket connection established.');
        });
    }

    async listenForFlowStatusEvents(): Promise<void> {
        try {
            await this.rabbitMQService.connect();
            console.log("Connected to RabbitMQ");
            
            await this.rabbitMQService.consumeEvents("flowstatus_created", (eventData) =>
                this.handleFlowStatusCreatedEvent(eventData)
            );
            console.log("Started listening for flowstatus_created events");
        } catch (error) {
            console.error("Error while listening for flowstatus_created events:", error);
        }
    }

    async handleFlowStatusCreatedEvent(eventData: any): Promise<void> {
        try {
            // Broadcast to all connected WebSocket clients
            this.wss.clients.forEach((client) => {
                if (client.readyState === WebSocket.OPEN) {
                    client.send(JSON.stringify(eventData));
                }
            });
        } catch (error) {
            console.error("Error processing Flow status created event:", error);
        }
    }

	async getAllFlowStatusesEvent(req: Request, res: Response): Promise<void> {
        try {
            await this.rabbitMQService.connect();
            await this.rabbitMQService.consumeRPCEvents("getAllFlowStatuses", "", (eventData) =>
                res.status(200).json(eventData)
            );

        } catch (error) {
            console.error("Error processing Get All Flow statuses event:", error);
            await this.rabbitMQService.restartConnection();
            res.status(404).json({ message: "No All Flow Statuses Event found" });
        }
    }
}

export default FlowController;