using System.Net.Mime;
using System.Text.Json.Serialization;

using MassTransit;
using MassTransitRabbitMQSample.AppService.MessageHeaders;
using MassTransitRabbitMQSample.Message.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//MassTransit
builder.Services.AddMassTransit((IBusRegistrationConfigurator registrationConfigurator) =>
{
	registrationConfigurator.AddConsumer<NormalMessageHeader>();
	registrationConfigurator.AddConsumer<FailMessageHeader>();
	registrationConfigurator.AddConsumer<FailFixeMessageHeader>();
	registrationConfigurator.AddConsumer<FaultMessageHeader>();
	registrationConfigurator.AddConsumer<FaultConsumerMessageHeader>();
	registrationConfigurator.AddConsumer<DiscardFaultedMessageHeader>();

	registrationConfigurator.UsingRabbitMq((IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator) =>
	{
		configurator.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")!));

		var directExchangeName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", "Direct");
		var normalMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(NormalMessage));
		configurator.ReceiveEndpoint(
			queueName: normalMessageQueueName,
			configureEndpoint: (IRabbitMqReceiveEndpointConfigurator endpointConfigurator) =>
			{
				endpointConfigurator.ExchangeType = ExchangeType.Direct;
				endpointConfigurator.Bind(
					exchangeName: directExchangeName,
					c => c.ExchangeType = ExchangeType.Direct);
				endpointConfigurator.Consumer<NormalMessageHeader>(context);
			});

		var failMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(FailMessage));
		configurator.ReceiveEndpoint(
			queueName: failMessageQueueName,
			configureEndpoint: (IRabbitMqReceiveEndpointConfigurator endpointConfigurator) =>
			{
				endpointConfigurator.ExchangeType = ExchangeType.Direct;
				endpointConfigurator.Bind(
					exchangeName: directExchangeName,
					c => c.ExchangeType = ExchangeType.Direct);
				endpointConfigurator.Consumer<FailMessageHeader>(context);
			});

		var failFixeMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", "Fixe");
		configurator.ReceiveEndpoint(
			queueName: failFixeMessageQueueName,
			configureEndpoint: (IRabbitMqReceiveEndpointConfigurator endpointConfigurator) =>
			{
				endpointConfigurator.ExchangeType = ExchangeType.Direct;
				endpointConfigurator.Bind(
					exchangeName: directExchangeName,
					c => c.ExchangeType = ExchangeType.Direct);
				endpointConfigurator.Consumer<FailFixeMessageHeader>(context);
			});

		var faultMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(FaultMessage));
		configurator.ReceiveEndpoint(
			queueName: faultMessageQueueName,
			configureEndpoint: (IRabbitMqReceiveEndpointConfigurator endpointConfigurator) =>
			{
				endpointConfigurator.ExchangeType = ExchangeType.Direct;
				endpointConfigurator.Bind(
					exchangeName: directExchangeName,
					c => c.ExchangeType = ExchangeType.Direct);
				endpointConfigurator.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)));
				endpointConfigurator.Consumer<FaultMessageHeader>(context);
				endpointConfigurator.Consumer<FaultConsumerMessageHeader>(context);
				endpointConfigurator.DiscardFaultedMessages();
			});

		var discardFaultedMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(DiscardFaultedMessage));
		configurator.ReceiveEndpoint(
			queueName: discardFaultedMessageQueueName,
			configureEndpoint: (IRabbitMqReceiveEndpointConfigurator endpointConfigurator) =>
			{
				endpointConfigurator.ExchangeType = ExchangeType.Direct;
				endpointConfigurator.Bind(
					exchangeName: directExchangeName,
					c => c.ExchangeType = ExchangeType.Direct);
				endpointConfigurator.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)));
				endpointConfigurator.Consumer<DiscardFaultedMessageHeader>(context);
				endpointConfigurator.DiscardFaultedMessages();
			});
	});
});

builder.Services.AddSingleton(TimeProvider.System);

builder.Services
	.AddSingleton(sp =>
	{
		var factory = new ConnectionFactory
		{
			Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")!),
		};
		return factory.CreateConnectionAsync().GetAwaiter().GetResult();
	})
	.AddHealthChecks()
	.AddRabbitMQ();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	_ = app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();
