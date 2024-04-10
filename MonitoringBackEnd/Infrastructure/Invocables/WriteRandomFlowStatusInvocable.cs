using System;
using System.Threading.Tasks;
using Infrastructure.Services;
using Coravel.Invocable;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace Infrastructure.Invocables
{
    public class WriteRandomFlowStatusInvocable : IInvocable
    {
        private readonly InfluxDBService _influxDBService;
        private static readonly Random _random = new Random();

        public WriteRandomFlowStatusInvocable(InfluxDBService influxDBService)
        {
            _influxDBService = influxDBService;
        }

        public Task Invoke()
        {
            _influxDBService.Write(write =>
            {
                var point = PointData.Measurement("flowStatus")
                    .Tag("flow", "test-flow")
                    .Field("value", _random.Next(0, 5))
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                write.WritePoint(point, "poc-bucket", "fourtop-ict");
            });

            Console.WriteLine(Task.CompletedTask);
            return Task.CompletedTask;
        }
    }
}