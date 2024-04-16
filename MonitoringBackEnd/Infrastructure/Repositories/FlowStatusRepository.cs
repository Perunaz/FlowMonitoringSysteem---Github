using Core.Domain;
using DomainServices.IRepositories;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Repositories
{
	public class FlowStatusRepository : IFlowStatusRepository
	{
		private readonly InfluxDBService _influxDBService;

		public FlowStatusRepository(InfluxDBService influxDBService)
		{
			_influxDBService = influxDBService;
		}

        public async Task<IEnumerable<(FlowStatus, string)>> GetAll()
        {
            var results = await _influxDBService.QueryAsync(async query =>
            {
                var flux = "from(bucket:\"poc-bucket\") |> range(start: 0)";
                var tables = await query.QueryAsync(flux, "fourtop-ict");
                return tables.SelectMany(table =>
                table.Records.Select(record =>
                    {
                        var flowStatus = new FlowStatus(
                            Guid.NewGuid(),
                            record.GetValueByKey("name").ToString(),
                            int.Parse(record.GetValueByKey("category").ToString()),
                            int.Parse(record.GetValue().ToString())
                        );

                        return (FlowStatus: flowStatus, Time: record.GetTime().ToString());
                    })
                );
            });

            Console.WriteLine(results.ToArray()[0]);
            return results;
        }

        public async Task<FlowStatus> WriteFlowStatus(FlowStatus flowStatus)
		{
			await Task.Run(() =>
			{
				_influxDBService.Write(write =>
				{
					var point = PointData.Measurement("flowStatus")
						.Tag("flow", flowStatus.FlowId.ToString())
						.Field("name", flowStatus.FlowName)
						.Field("category", flowStatus.FlowCategory)
						.Field("value", flowStatus.StateCode)
                        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

					write.WritePoint(point, "poc-bucket", "fourtop-ict");
				});
			});

			return flowStatus;
		}
	}
}
