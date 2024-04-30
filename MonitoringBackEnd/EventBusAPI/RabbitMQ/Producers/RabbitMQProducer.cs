using EventBusAPI.RabbitMQ.IProducers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Channels;

namespace EventBusAPI.RabbitMQ.Producers
{
	public class RabbitMQProducer : IRabbitMQProducer
	{
		private string _hostName;
		private int _port;
        private IConnection connection;
        private IModel channel;

        public RabbitMQProducer(IConfiguration configuration)
		{
			_hostName = configuration.GetValue<string>("RabbitMQ:HostName");
			_port = configuration.GetValue<int>("RabbitMQ:Port");
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
