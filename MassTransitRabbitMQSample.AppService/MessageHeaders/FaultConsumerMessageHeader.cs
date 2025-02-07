using MassTransit;
using MassTransitRabbitMQSample.Message.Models;

namespace MassTransitRabbitMQSample.AppService.MessageHeaders;

public class FaultConsumerMessageHeader(
	ILogger<FaultConsumerMessageHeader> logger,
	TimeProvider timeProvider) : IConsumer<Fault<FaultMessage>>
{
	public Task Consume(ConsumeContext<Fault<FaultMessage>> context)
	{
		logger.LogError("{logError}", new
		{
			LogAt = timeProvider.GetUtcNow().ToString("u"),
			context.Message.Message
		});

		return Task.CompletedTask;
	}
}
