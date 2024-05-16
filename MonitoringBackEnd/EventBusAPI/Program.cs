using DomainServices.IRepositories;
using DomainServices.Services.Implementation;
using DomainServices.Services.Interface;
using EventBusAPI.RabbitMQ.IProducers;
using EventBusAPI.RabbitMQ.Producers;
using Infrastructure.Services;
using Infrastructure.Repositories;
using Coravel;
using Core.Domain.Flow;
using EventBusAPI.Controllers;
using EventBusAPI.RabbitMQ.IReceiver;
using EventBusAPI.RabbitMQ.Reveivers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Create WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json or environment variables
var configuration = builder.Configuration;

// Add other services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<InfluxDBService>();
builder.Services.AddScoped<FlowStatusController>();
builder.Services.AddScheduler();
builder.Services.AddScoped<IFlowStatusRepository, FlowStatusRepository>();
builder.Services.AddScoped<IFlowStatusService, FlowStatusService>();
builder.Services.AddScoped<IRabbitMQProducer, RabbitMQExchangeProducer>();
builder.Services.AddScoped<IRabbitMQRPCProducer, RabbitMQRPCProducerFlowStatus>();

builder.Services.AddScoped<TokenManagementService>();
builder.Services.AddScoped<PowerAutomateAPIService>();

// Build the WebApplication
var app = builder.Build();

// Middleware configuration
app.UseHttpsRedirection();

// Apply authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Connect to RabbitMQ at startup
app.Lifetime.ApplicationStarted.Register(() =>
{
	using var scope = app.Services.CreateScope();
	var rpcProducer = scope.ServiceProvider.GetRequiredService<IRabbitMQRPCProducer>();
	rpcProducer.Connect(); // Establish the RabbitMQ connection
});

// Clean up RabbitMQ connection upon stopping
app.Lifetime.ApplicationStopping.Register(() =>
{
	using var scope = app.Services.CreateScope();
	var rpcProducer = scope.ServiceProvider.GetRequiredService<IRabbitMQRPCProducer>();
	rpcProducer.Disconnect(); // Clean up the RabbitMQ connection
});

// Schedule recurring tasks
var random = new Random();
app.Services.UseScheduler(scheduler =>
{
	scheduler.Schedule(async () =>
	{
		using var scope = app.Services.CreateScope();
		var serviceProvider = scope.ServiceProvider;

		var controller = serviceProvider.GetRequiredService<FlowStatusController>();
		var powerAutomateAPIService = serviceProvider.GetRequiredService<PowerAutomateAPIService>();
		try
		{
			var flowsString = await powerAutomateAPIService.GetResourceFromPowerAutomateFlowsAsync();
			dynamic flows = JObject.Parse(flowsString);

			foreach (var flow in flows.value)
			{
				// Get last run of the 
				var flowName = flow.name.ToString();
				var lastRun = JObject.Parse(await powerAutomateAPIService.GetResourceFromPowerAutomateFlowLastRunAsync(flowName));

				// Extract required information
				var flowId = flow.name.ToString();
				var displayName = flow.properties.displayName.ToString();
				var lastRunStatus = lastRun.value[0].properties.status.ToString();

				// Create object and add to list
				var newFlowStatus = new FlowStatus(flowId, displayName, lastRunStatus);
				controller.AddFlowStatus(newFlowStatus);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	})
	.EveryFifteenMinutes(); // Schedule to run every 5 seconds
});

// Start the application
app.Run();