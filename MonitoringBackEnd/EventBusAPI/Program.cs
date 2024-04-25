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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<InfluxDBService>();
builder.Services.AddScoped<FlowStatusController>();
builder.Services.AddScheduler();

builder.Services.AddScoped<IFlowStatusRepository, FlowStatusRepository>();

builder.Services.AddScoped<IFlowStatusService, FlowStatusService>();

builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
builder.Services.AddScoped<IRabbitMQRPCProducer, RabbitMQRPCProducerFlowStatus>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Connect to RabbitMQ at startup
app.Lifetime.ApplicationStarted.Register(() =>
{
	using var scope = app.Services.CreateScope();
	var rpcProducer = scope.ServiceProvider.GetRequiredService<IRabbitMQRPCProducer>();
	rpcProducer.Connect();  // Establish the RabbitMQ connection
});

// Create a scope for disconnecting from RabbitMQ
app.Lifetime.ApplicationStopping.Register(() =>
{
	using var scope = app.Services.CreateScope();
	var rpcProducer = scope.ServiceProvider.GetRequiredService<IRabbitMQRPCProducer>();
	rpcProducer.Disconnect();  // Clean up the RabbitMQ connection
});

app.Services.UseScheduler(scheduler =>
{
	scheduler.Schedule(() =>
	{
		using var scope = app.Services.CreateScope();
		var serviceProvider = scope.ServiceProvider;

		var flowStatus = new FlowStatus("3fa85f64-5717-4562-b3fc-2c963f66afa6", 1);

		var controller = serviceProvider.GetRequiredService<FlowStatusController>();
		controller.AddFlowStatus(flowStatus);
	})
	.EveryFiveSeconds();
});

app.Run();
