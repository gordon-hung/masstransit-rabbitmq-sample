namespace MassTransitRabbitMQSample.Message.Models;

public record FailMessage
{
	public Guid Id { get; init; }
	public DateTimeOffset MessageAt { get; init; }
}
