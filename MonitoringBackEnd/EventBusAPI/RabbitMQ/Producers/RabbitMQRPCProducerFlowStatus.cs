using EventBusAPI.Controllers;
using EventBusAPI.RabbitMQ.IProducers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace EventBusAPI.RabbitMQ.Producers
{
	public class RabbitMQRPCProducerFlowStatus : IRabbitMQRPCProducer
	{
		private string _hostName;
		private int _port;
		private IConnection connection;
		private IModel channel;

		private FlowStatusController _flowStatusController;

		public RabbitMQRPCProducerFlowStatus(IConfiguration configuration, FlowStatusController flowStatusController)
		{
			_hostName = configuration.GetValue<string>("RabbitMQ:HostName");
			_port = configuration.GetValue<int>("RabbitMQ:Port");

			_flowStatusController = flowStatusController;
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

			channel.QueueDeclare(queue: "getAllFlowStatuses",
					 durable: false,
					 exclusive: false,
					 autoDelete: false,
					 arguments: null);

			channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

			var consumer = new EventingBasicConsumer(channel);
			channel.BasicConsume(queue: "getAllFlowStatuses",
								 autoAck: false,
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

		public async void OnMessageReceived(object model, BasicDeliverEventArgs ea)
		{
			string response = string.Empty;

			var props = ea.BasicProperties;
			var replyProps = channel.CreateBasicProperties();
			replyProps.CorrelationId = props.CorrelationId;

			try
			{
				var flowstatuses = await _flowStatusController.GetAllFlowStatuses();
				var json = JsonConvert.SerializeObject(flowstatuses);
				response = json;
			}
			catch (Exception e)
			{
				Console.WriteLine($" [.] {e.Message}");
				response = String.Empty;
			}
			finally
			{
				var responseBytes = Encoding.UTF8.GetBytes(response);
				channel.BasicPublish(exchange: string.Empty,
									 routingKey: props.ReplyTo,
									 basicProperties: replyProps,
									 body: responseBytes);
				channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
			}
		}

	}
}
