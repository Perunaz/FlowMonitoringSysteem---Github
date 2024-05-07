using RabbitMQ.Client.Events;

namespace EventBusAPI.RabbitMQ.IReceiver
{
	public interface IRabbitMQReceiver
	{
		public void Connect();
		public void Disconnect();
		void OnMessageReceived(object model, BasicDeliverEventArgs ea);
	}
}
