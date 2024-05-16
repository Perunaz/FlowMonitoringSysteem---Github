using Core.Domain.Flow;
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

		public async Task<IEnumerable<FlowStatusModel>> GetAll()
		{
			var results = await _influxDBService.QueryAsync(async query =>
			{
				var currentTime = DateTime.UtcNow;
				var fiveMinutesAgo = currentTime.AddMinutes(-60);

				// Create the Flux query to get the last 5 minutes of data
				var flux = $"from(bucket:\"poc-bucket\") |> range(start: {fiveMinutesAgo:o}, stop: {currentTime:o})";

				var tables = await query.QueryAsync(flux, "fourtop-ict");
				Console.WriteLine(tables);

				return tables.SelectMany(table =>
					table.Records.Select(record =>
					{
						var flowStatus = new FlowStatusModel(
							record.GetValueByKey("flowId").ToString(),
							record.GetValueByKey("name").ToString(),
							record.GetValue().ToString(),
							record.GetTime().ToString()
						);

						return flowStatus;
					})
				);
			});

			return results;
		}

		public async Task<FlowStatus> WriteFlowStatus(FlowStatus flowStatus)
		{
			await Task.Run(() =>
			{
				_influxDBService.Write(write =>
				{
					var point = PointData.Measurement("flowStatus")
						.Tag("flowId", flowStatus.FlowId.ToString())
						.Tag("name", flowStatus.DisplayName)
						.Field("value", flowStatus.Status)
                        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

					write.WritePoint(point, "poc-bucket", "fourtop-ict");
				});
			});

			return flowStatus;
		}
	}
}
