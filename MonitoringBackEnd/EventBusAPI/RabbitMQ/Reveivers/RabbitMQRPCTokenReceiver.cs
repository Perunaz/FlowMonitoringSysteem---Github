using EventBusAPI.RabbitMQ.IReceiver;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EventBusAPI.RabbitMQ.Reveivers
{
    public class RabbitMQTokenReceiver : IRabbitMQReceiver
    {
        private string _hostName;
        private int _port;
        private IConnection connection;
        private IModel channel;
        private TokenManagementService _tokenManagementService;

        public RabbitMQTokenReceiver(IConfiguration configuration, TokenManagementService tokenManagementService)
        {
            _hostName = configuration.GetValue<string>("RabbitMQ:HostName");
            _port = configuration.GetValue<int>("RabbitMQ:Port");
            _tokenManagementService = tokenManagementService;
        }

        public void Connect()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = _hostName,
                Port = _port,
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "tokens",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

			var consumer = new EventingBasicConsumer(channel);
			channel.BasicConsume(queue: "tokens",
								 autoAck: true,
								 consumer: consumer);

			consumer.Received += OnMessageReceived;
		}

		public void Disconnect()
        {
            channel = null;

            if (connection.IsOpen)
            {
                connection.Close();
            }

            connection.Dispose();
            connection = null;
        }

		public void OnMessageReceived(object model, BasicDeliverEventArgs ea)
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			var jsonMessage = JsonDocument.Parse(message);

            Console.WriteLine(jsonMessage.RootElement.ToString());
            Console.WriteLine(message);

			var accessToken = jsonMessage.RootElement.GetProperty("accessToken").GetString();
			var refreshToken = jsonMessage.RootElement.GetProperty("refreshToken").GetString();
			var expiry = jsonMessage.RootElement.GetProperty("expiry").GetInt32();

            _tokenManagementService.UseNewtoken(accessToken, refreshToken, expiry);
		}
	}
}
