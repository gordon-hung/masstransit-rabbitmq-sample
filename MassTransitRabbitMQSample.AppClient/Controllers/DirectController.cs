using Aoxe.SequentialGuid;
using MassTransit;
using MassTransitRabbitMQSample.Message.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitRabbitMQSample.AppClient.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectController : ControllerBase
{
	[HttpPost("Normal")]
	public Task NormalMessagePublish(
		[FromServices] TimeProvider timeProvider,
		[FromServices] IBus bus)
		=> bus.Publish(
			message: new NormalMessage
			{
				Id = SequentialGuidHelper.GenerateComb(),
				MessageAt = timeProvider.GetUtcNow()
			},
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("Fault")]
	public Task FaultMessagePublish(
		[FromServices] TimeProvider timeProvider,
		[FromServices] IBus bus)
		=> bus.Publish(
			message: new FaultMessage
			{
				Id = SequentialGuidHelper.GenerateComb(),
				MessageAt = timeProvider.GetUtcNow()
			},
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("Fail")]
	public Task FailMessagePublish(
		[FromServices] TimeProvider timeProvider,
		[FromServices] IBus bus)
		=> bus.Publish(
			message: new FailMessage
			{
				Id = SequentialGuidHelper.GenerateComb(),
				MessageAt = timeProvider.GetUtcNow()
			},
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("Discard-Faulted")]
	public Task DiscardFaultedMessagesPublish(
		[FromServices] TimeProvider timeProvider,
		[FromServices] IBus bus)
		=> bus.Publish(
			message: new DiscardFaultedMessage
			{
				Id = SequentialGuidHelper.GenerateComb(),
				MessageAt = timeProvider.GetUtcNow()
			},
			cancellationToken: HttpContext.RequestAborted);
}
