using MassTransit;
using MassTransitRabbitMQSample.Message.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Net.Mime;
using System.Text.Json.Serialization;

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
builder.Services.AddMassTransit((IBusRegistrationConfigurator registrationConfigurator)
	=> registrationConfigurator.UsingRabbitMq((IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator) =>
	{
		configurator.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")!));

		var normalMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(NormalMessage));
		configurator.Message<NormalMessage>(c => c.SetEntityName(normalMessageQueueName));
		configurator.Publish<NormalMessage>(c => c.ExchangeType = ExchangeType.Direct);

		var faultMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(FaultMessage));
		configurator.Message<FaultMessage>(c => c.SetEntityName(faultMessageQueueName));
		configurator.Publish<FaultMessage>(c => c.ExchangeType = ExchangeType.Direct);

		var failMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(FailMessage));
		configurator.Message<FailMessage>(c => c.SetEntityName(failMessageQueueName));
		configurator.Publish<FailMessage>(c => c.ExchangeType = ExchangeType.Direct);

		var discardFaultedMessageQueueName = string.Concat(builder.Environment.EnvironmentName, ".", "Sample", ".", nameof(DiscardFaultedMessage));
		configurator.Message<DiscardFaultedMessage>(c => c.SetEntityName(discardFaultedMessageQueueName));
		configurator.Publish<DiscardFaultedMessage>(c => c.ExchangeType = ExchangeType.Direct);

	}));

builder.Services.AddSingleton(TimeProvider.System);

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

app.Run();
