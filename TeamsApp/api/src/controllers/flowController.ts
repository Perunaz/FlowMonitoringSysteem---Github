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
                "themeColor": "1a1246",
                "summary": "Flow Failed: " + eventData.FlowId,
                "sections": [{
                    "activityTitle": "A Flow Has Failed",
                    "activityImage": "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Microsoft_Power_Automate.svg/2048px-Microsoft_Power_Automate.svg.png",
                    "facts": [{
                        "name": "Flow Name",
                        "value": eventData.DisplayName
                    },{
                        "name": "Status",
                        "value": eventData.Status
                    }],
                    "markdown": true
                    
                }],
                "potentialAction": [{
                    "@type": "OpenUri",
                    "name": "Go To Flow",
                    "targets": [{
                        "os": "default",
                        "uri": `https://make.powerautomate.com/environments/Default-${process.env.TENANT_ID}/flows/${eventData.FlowId}/details`
                    }]
                }]
            };

            // The webhook URL
            const webhookUrl = process.env.WEBHOOK_URL;

            if (!webhookUrl) {
                throw new Error("WEBHOOK_URL environment variable is not set.");
            }

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