using EventBusAPI.RabbitMQ.IProducers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EventBusAPI.RabbitMQ.Producers
{
	public class RabbitMQProducer : IRabbitMQProducer
	{
		private readonly string _url;

		public RabbitMQProducer(IConfiguration configuration)
		{
			_url = configuration.GetValue<string>("RabbitMQ:Url");
		}

		public void PublishEvent<T>(T message, string queueName, string routingKey, string exchange)
		{
			//Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
			var factory = new ConnectionFactory
			{
				HostName = _url
			};
			//Create the RabbitMQ connection using connection factory details as i mentioned above
			var connection = factory.CreateConnection();
			//Here we create channel with session and model
			using
			var channel = connection.CreateModel();
			//declare the queue after mentioning name and a few property related to that
			channel.QueueDeclare(queueName, exclusive: false);
			//Serialize the message
			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);
			//put the data on to the product queue
			channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body);
		}
	}
}
