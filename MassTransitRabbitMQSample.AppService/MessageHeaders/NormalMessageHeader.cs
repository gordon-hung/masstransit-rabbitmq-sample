using MassTransit;
using MassTransitRabbitMQSample.Message.Models;

namespace MassTransitRabbitMQSample.AppService.MessageHeaders;

public class NormalMessageHeader(
	ILogger<NormalMessageHeader> logger,
	TimeProvider timeProvider) : IConsumer<NormalMessage>
{
	public Task Consume(ConsumeContext<NormalMessage> context)
	{
		logger.LogInformation("{logInformation}", new
		{
			LogAt = timeProvider.GetUtcNow().ToString("u"),
			context.Message
		});

		return Task.CompletedTask;

	}
}
