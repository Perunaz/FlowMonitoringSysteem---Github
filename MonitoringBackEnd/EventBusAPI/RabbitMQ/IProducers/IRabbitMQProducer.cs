namespace EventBusAPI.RabbitMQ.IProducers
{
	public interface IRabbitMQProducer
	{
		public void Connect(string queueName);
		public void Disconnect();
		public void SendMessage<T>(T message, string queueName);

	}
}
