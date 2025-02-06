namespace MassTransitRabbitMQSample.Message.Models;

public record NormalMessage
{
	public Guid Id { get; init; }
	public DateTimeOffset MessageAt { get; init; }
}
