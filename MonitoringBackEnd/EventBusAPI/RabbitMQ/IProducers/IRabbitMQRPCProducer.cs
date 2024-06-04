using RabbitMQ.Client.Events;

namespace EventBusAPI.RabbitMQ.IProducers
{
	public interface IRabbitMQRPCProducer
	{
		public void Connect();
		public void Disconnect();
		void OnMessageReceived(object model, BasicDeliverEventArgs ea);
	}
}
