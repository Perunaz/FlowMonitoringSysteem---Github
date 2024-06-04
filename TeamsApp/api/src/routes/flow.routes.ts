import express, { Router, Request, Response } from "express";
import RabbitMQService from "../../rabbitMQService";
import FlowController from "../controllers/flowController";

class FlowRouter {
	private router: Router;
    private flowController: FlowController;

    constructor(
        flowController: FlowController
    ) {
        this.flowController = flowController
        this.router = express.Router();
        this.initializeRoutes();
    }

    private initializeRoutes(): void {
        this.router.get("/", async (req: Request, res: Response) => {
            try {
                await this.flowController.getAllFlowStatusesEvent(req, res);
            } catch (error) {
                console.error("Error fetching the last event:", error);
                res.status(500).json({ message: "Internal server error" });
            }
		});
    }

	public getRouter(): Router {
		return this.router;
	}
}

export default FlowRouter;