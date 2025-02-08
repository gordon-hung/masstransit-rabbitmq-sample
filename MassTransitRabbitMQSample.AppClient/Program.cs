using System.Net.Mime;
using System.Text.Json.Serialization;

using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using MassTransitRabbitMQSample.Message.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

builder.Services.AddOpenTelemetry()
	.ConfigureResource(resource => resource
	.AddService(builder.Configuration["SERVICE_NAME"]!))
	.UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(builder.Configuration["OTLP_ENDPOINT_URL"]!))
	.WithMetrics(metrics => metrics
		//.SetResourceBuilder(ResourceBuilder.CreateDefault()
		//.AddEnvironmentVariableDetector())
		.AddMeter("MassTransitRabbitMQSample.")
		.AddMeter(InstrumentationOptions.MeterName)
		.AddPrometheusExporter()
		.AddConsoleExporter()
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation())
	.WithTracing(tracing => tracing
		 //.SetResourceBuilder(ResourceBuilder.CreateDefault()
		 //.AddEnvironmentVariableDetector())
		 .AddSource(DiagnosticHeaders.DefaultListenerName)
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddAspNetCoreInstrumentation(options => options.Filter = (httpContext) =>
				!httpContext.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.Value!.Equals("/api/events/raw", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.Value!.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/_vs", StringComparison.OrdinalIgnoreCase)))
	.WithLogging(logging => logging
		.AddConsoleExporter());

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
	.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
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
