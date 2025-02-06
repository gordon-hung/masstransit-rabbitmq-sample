using MassTransit;
using MassTransitRabbitMQSample.Message.Models;

namespace MassTransitRabbitMQSample.AppService.MessageHeaders;

public class DiscardFaultedMessageHeader(
	ILogger<DiscardFaultedMessageHeader> logger,
	TimeProvider timeProvider) : IConsumer<DiscardFaultedMessage>
{
	public Task Consume(ConsumeContext<DiscardFaultedMessage> context)
	{
		logger.LogInformation("{logInformation}", new
		{
			LogAt = timeProvider.GetUtcNow().ToString("u"),
			context.Message
		});

		throw new NotImplementedException();

	}
}