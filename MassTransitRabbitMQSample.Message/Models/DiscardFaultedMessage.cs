namespace MassTransitRabbitMQSample.Message.Models;

public record DiscardFaultedMessage
{
	public Guid Id { get; init; }
	public DateTimeOffset MessageAt { get; init; }
}
