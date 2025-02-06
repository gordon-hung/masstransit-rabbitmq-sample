using MassTransit;
using MassTransitRabbitMQSample.Message.Models;

namespace MassTransitRabbitMQSample.AppService.MessageHeaders;

public class FaultMessageHeader(
	ILogger<FaultMessageHeader> logger,
	TimeProvider timeProvider) : IConsumer<FaultMessage>
{
	public Task Consume(ConsumeContext<FaultMessage> context)
	{
		logger.LogInformation("{logInformation}", new
		{
			LogAt = timeProvider.GetUtcNow().ToString("u"),
			context.Message
		});

		throw new NotImplementedException();

	}
}