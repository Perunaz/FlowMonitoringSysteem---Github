import RabbitMQService from "../../rabbitMQService";
import { Request, Response } from "express";

class TokenController {
    private rabbitMQService: RabbitMQService;

    constructor(rabbitMQService: RabbitMQService) {
        this.rabbitMQService = rabbitMQService;
    }

    async postTokensEvent(
        req: Request,
        res: Response
      ): Promise<void> {
        const { accessToken } = req.body;
      
        try {
          // Check if RabbitMQ service is connected, if not, establish connection
          if (!this.rabbitMQService.isConnected()) {
            await this.rabbitMQService.connect();
          }
      
          // Send tokens to the specified queue (use a method in your RabbitMQ service)
          await this.rabbitMQService.sendTokens(accessToken);
      
          // Return success response
          res.status(200).json({ message: 'Tokens posted successfully.' });
        } catch (error) {
          console.error('Error posting tokens event:', error);
          
          await this.rabbitMQService.restartConnection();
      
          // Return error response
          res.status(500).json({ message: 'Failed to post tokens event.' });
        }
    }

    setAccessToken(accessToken: string) {
      console.log('setAccessToken');
      this.rabbitMQService.setAccessToken(accessToken);
    }
}

export default TokenController;