using EventBusAPI.RabbitMQ.IProducers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Channels;

namespace EventBusAPI.RabbitMQ.Producers
{
	public class RabbitMQExchangeProducer : IRabbitMQProducer
	{
		private string _hostName;
		private int _port;
        private IConnection connection;
        private IModel channel;

        public RabbitMQExchangeProducer()
		{
			_hostName = Environment.GetEnvironmentVariable("CLIENT_ID");
			_port = Int32.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"));

        }

        public void Connect(string exchangeName)
		{
			ConnectionFactory factory = new ConnectionFactory
			{
                HostName = _hostName,
                Port = _port,
            };

			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
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

		public void SendMessage<T>(T message, string exchangeName)
		{
            var json = JsonConvert.SerializeObject(message);
			byte[] messageBody = Encoding.UTF8.GetBytes(json);
			channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, basicProperties: null, body: messageBody);
		}
	}
}
