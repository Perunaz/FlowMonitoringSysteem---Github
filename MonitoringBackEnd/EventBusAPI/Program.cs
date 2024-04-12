using DomainServices.IRepositories;
using DomainServices.Services.Implementation;
using DomainServices.Services.Interface;
using EventBusAPI.RabbitMQ.IProducers;
using EventBusAPI.RabbitMQ.Producers;
using Infrastructure.Services;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InfluxDBService>();

builder.Services.AddScoped<IFlowStatusRepository, FlowStatusRepository>();

builder.Services.AddScoped<IFlowStatusService, FlowStatusService>();

builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
