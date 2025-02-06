using MassTransit;
using MassTransitRabbitMQSample.Message.Models;

namespace MassTransitRabbitMQSample.AppService.MessageHeaders;

public class FailFixeMessageHeader(
	ILogger<FailFixeMessageHeader> logger,
	TimeProvider timeProvider) : IConsumer<FailMessage>
{
	public Task Consume(ConsumeContext<FailMessage> context)
	{
		logger.LogWarning("{logInformation}", new
		{
			LogAt = timeProvider.GetUtcNow().ToString("u"),
			context.Message
		});

		return Task.CompletedTask;

	}
}
