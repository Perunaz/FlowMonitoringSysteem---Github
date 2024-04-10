namespace EventBusAPI.RabbitMQ.IProducers
{
	public interface IRabbitMQProducer
	{
		public void PublishEvent<T>(T message, string queueName, string routingKey, string exchange);
	}
}
