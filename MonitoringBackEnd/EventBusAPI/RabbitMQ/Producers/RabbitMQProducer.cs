using EventBusAPI.RabbitMQ.IProducers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EventBusAPI.RabbitMQ.Producers
{
	public class RabbitMQProducer : IRabbitMQProducer
	{
		private readonly string _hostName;
		private readonly int _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;

        public RabbitMQProducer(IConfiguration configuration)
		{
            _hostName = configuration.GetValue<string>("RabbitMQ:HostName");
			_port = configuration.GetValue<int>("RabbitMQ:Port");
            _userName = configuration.GetValue<string>("RabbitMQ:UserName");
            _password = configuration.GetValue<string>("RabbitMQ:Password");

            _factory = new ConnectionFactory
            {
                HostName = _hostName,
                Port = _port,
                UserName = _userName,
                Password = _password,
            };
        }

        public void PublishEvent<T>(T message, string queueName, string routingKey, string exchange)
		{
            // Check if connection is null or closed, then create a new one
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = _factory.CreateConnection();
            }

            // Create the RabbitMQ channel using the existing connection
            using (var channel = _connection.CreateModel())
            {
                // Declare the queue
                channel.QueueDeclare(queueName, exclusive: false);

                // Serialize the message
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                // Publish the message
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body);
            }
        }
	}
}
