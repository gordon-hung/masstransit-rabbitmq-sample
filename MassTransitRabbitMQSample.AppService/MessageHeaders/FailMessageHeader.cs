using MassTransit;
using MassTransitRabbitMQSample.Message.Models;

namespace MassTransitRabbitMQSample.AppService.MessageHeaders;

public class FailMessageHeader(
	ILogger<FailMessageHeader> logger,
	TimeProvider timeProvider) : IConsumer<FailMessage>
{
	public Task Consume(ConsumeContext<FailMessage> context)
	{
		logger.LogInformation("{logInformation}", new
		{
			LogAt = timeProvider.GetUtcNow().ToString("u"),
			context.Message
		});

		throw new NotImplementedException();

	}
}