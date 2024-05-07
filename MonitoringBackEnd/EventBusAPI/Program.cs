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

		// Set a probability threshold for status to be 1
		const int probabilityThreshold = 95;

		// Function to generate random status with 95% chance of being 1
		int GetRandomStatus()
		{
			return random.Next(0, 101) <= probabilityThreshold ? 1 : 0;
		}

		// Generate random status with 95% probability of being 1
		var flowStatus1 = new FlowStatus("3fa85f64-5717-4562-b3fc-2c963f66afa6", GetRandomStatus());
		var flowStatus2 = new FlowStatus("12345678-1234-1234-1234-1234567890ab", GetRandomStatus());
		var flowStatus3 = new FlowStatus("a82c7ef9-4a6e-4c2f-8cba-71444a7e153f", GetRandomStatus());

		var controller = serviceProvider.GetRequiredService<FlowStatusController>();

		// Add FlowStatus instances to the controller
		controller.AddFlowStatus(flowStatus1);
		controller.AddFlowStatus(flowStatus2);
		controller.AddFlowStatus(flowStatus3);

		var powerAutomateAPIService = scope.ServiceProvider.GetRequiredService<PowerAutomateAPIService>();
		try
		{
			var resource = await powerAutomateAPIService.GetResourceFromPowerAutomateFlowsAsync();
			Console.WriteLine(resource.ToString());
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	})
	.EveryFiveSeconds(); // Schedule to run every 5 seconds
});

// Start the application
app.Run();