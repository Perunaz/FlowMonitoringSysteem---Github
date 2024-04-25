import RabbitMQService from "../../rabbitMQService"
import { Request, Response } from "express";

class FlowController {
    private rabbitMQService: RabbitMQService;
    private lastFlowStatusCreatedEvent: any;

    constructor(rabbitMQService: RabbitMQService) {
        this.rabbitMQService = rabbitMQService;
        this.lastFlowStatusCreatedEvent = null;
		this.AllFlowStatusesEvent
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

	getAllFlowStatusesEvent(req: Request, res: Response): void {
        if (this.lastFlowStatusCreatedEvent) {
            res.status(200).json({
                message: "All Flow Statuses Event",
                event: this.lastFlowStatusCreatedEvent
            });
        } else {
            res.status(404).json({ message: "No Flow Status Created Event found" });
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