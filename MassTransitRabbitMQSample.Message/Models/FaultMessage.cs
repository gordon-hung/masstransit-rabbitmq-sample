namespace MassTransitRabbitMQSample.Message.Models;

public record FaultMessage
{
	public Guid Id { get; init; }
	public DateTimeOffset MessageAt { get; init; }
}
