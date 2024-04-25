import RabbitMQService from "../../rabbitMQService"
import { Request, Response } from "express";

class FlowController {
    private rabbitMQService: RabbitMQService;
    private lastFlowStatusCreatedEvent: any;

    constructor(rabbitMQService: RabbitMQService) {
        this.rabbitMQService = rabbitMQService;
        this.lastFlowStatusCreatedEvent = null;
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
            console.log("Flow status created event processed:", eventData);
            this.lastFlowStatusCreatedEvent = eventData;
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
            res.status(404).json({ message: "No All Flow Statuses Event found" });
        }
    }

    getLastFlowStatusCreatedEvent(req: Request, res: Response): void {
        if (this.lastFlowStatusCreatedEvent) {
            res.status(200).json({
                message: "Last Flow Status Created Event",
                event: this.lastFlowStatusCreatedEvent
            });
        } else {
            res.status(404).json({ message: "No Flow Status Created Event found" });
        }
    }
}

export default FlowController;