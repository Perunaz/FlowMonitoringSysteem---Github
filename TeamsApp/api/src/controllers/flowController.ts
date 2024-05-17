import RabbitMQService from "../../rabbitMQService";
import WebSocket from 'ws';
import { Request, Response } from "express";
import axios from "axios";

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
            
            await this.rabbitMQService.consumeEvents("flowstatus_changed", (eventData) =>
                this.handleFlowStatusCreatedEvent(eventData)
            );
            console.log("Started listening for flowstatus_changed events");
        } catch (error) {
            console.error("Error while listening for flowstatus_changed events:", error);
        }
    }

    async handleFlowStatusCreatedEvent(eventData: any): Promise<void> {
        try {
            // Log the event data being sent
            console.log("Sending event data to WebSocket clients:", eventData);
            // Prepare the data to be sent in the POST request
            const postData = {
                "@type": "MessageCard",
                "@context": "http://schema.org/extensions",
                "themeColor": "0076D7",
                "summary": `Flow Status: ${eventData.Status}`,
                "sections": [{
                    "activityTitle": "Flow Status Update",
                    "activitySubtitle": `Flow ID: ${eventData.FlowId}`,
                    "activityImage": "https://i.pinimg.com/originals/a0/c6/d6/a0c6d6d5bb928ad13d2ebcf7b1f7d602.png",
                    "facts": [{
                        "name": "Flow ID",
                        "value": eventData.FlowId
                    },{
                        "name": "Display Name",
                        "value": eventData.DisplayName
                    },{
                        "name": "Status",
                        "value": eventData.Status
                    }],
                    "markdown": true
                }]
            };

            // The webhook URL
            const webhookUrl = 'https://fourtopstaging.webhook.office.com/webhookb2/064dc41d-8e5d-4b81-8c52-9979418549f6@ee5fab9c-fc10-4fbf-b8f6-387bffbb5196/IncomingWebhook/55adce1ba6804b94908034c67abee66f/c5fa6eec-2c80-4123-a51e-7404f96da1ed';

            // Send the POST request to the webhook URL
            const response = await axios.post(webhookUrl, postData, {
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            console.log("Response from webhook:", response.data);
            
            // Broadcast to all connected WebSocket clients
            // this.wss.clients.forEach((client) => {
            //     if (client.readyState === WebSocket.OPEN) {
            //         client.send(JSON.stringify(eventData));
            //     }
            // });
        } catch (error) {
            console.error("Error processing Flow status created event:", error);
        }
    }

	async getAllFlowStatusesEvent(req: Request, res: Response): Promise<void> {
        try {
            if (!this.rabbitMQService.isConnected()) {
                await this.rabbitMQService.connect();
            }
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