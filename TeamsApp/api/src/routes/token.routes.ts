import express, { Router, Request, Response } from "express";
import RabbitMQService from "../../rabbitMQService";
import TokenController from "../controllers/tokenController";

class TokenRouter {
	private router: Router;
    private tokenController: TokenController;

    constructor(
        tokenController: TokenController
    ) {
        this.tokenController = tokenController
        this.router = express.Router();
        this.initializeRoutes();
    }

    private initializeRoutes(): void {
        this.router.post("/", async (req: Request, res: Response) => {
            try {
                await this.tokenController.postTokensEvent(req, res);
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

export default TokenRouter;