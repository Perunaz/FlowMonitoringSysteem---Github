using Infrastructure.Services;
using Coravel;
using Infrastructure.Invocables;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddSingleton<InfluxDBService>();
builder.Services.AddTransient<WriteRandomFlowStatusInvocable>();
builder.Services.AddScheduler();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<WriteRandomFlowStatusInvocable>()
        .EveryFiveSeconds();
});

app.Run();
