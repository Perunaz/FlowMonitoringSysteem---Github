using Core.Domain.Flow;
using DomainServices.IRepositories;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Infrastructure.IServices;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class FlowStatusRepository : IFlowStatusRepository
	{
		private readonly IInfluxDBService _influxDBService;

		public FlowStatusRepository(IInfluxDBService influxDBService)
		{
			_influxDBService = influxDBService;
		}

		public async Task<IEnumerable<FlowStatusModel>> GetAll()
        {
            var currentTime = DateTime.UtcNow;
            var thirtyMinutesAgo = currentTime.AddMinutes(-30);

            // Create the Flux query to get the last 30 minutes of data and pivot it
            var flux = $@"
						from(bucket: ""poc-bucket"")
							|> range(start: {thirtyMinutesAgo:o}, stop: {currentTime:o})
							|> filter(fn: (r) => r._measurement == ""flowStatus"")
							|> pivot(rowKey: [""_time""], columnKey: [""_field""], valueColumn: ""_value"")
				";

            var results = await _influxDBService.QueryAsync(async query =>
			{
				var tables = await query.QueryAsync(flux, "fourtop-ict");

				return tables.SelectMany(table =>
					table.Records.Select(record =>
					{
						var flowId = record.GetValueByKey("flowId")?.ToString();
						var name = record.GetValueByKey("name")?.ToString();
						var status = record.GetValueByKey("status")?.ToString();
						var successPercentage = record.GetValueByKey("successPercentage") != null ? Convert.ToInt32(record.GetValueByKey("successPercentage")) : 0;
						var runCount = record.GetValueByKey("runCount") != null ? Convert.ToInt32(record.GetValueByKey("runCount")) : 0;
						var time = record.GetTime().ToString();

						var flowStatus = new FlowStatusModel(
							flowId,
							name,
							status,
							successPercentage,
							runCount,
							time
						);

						return flowStatus;
					})
				);
			});

			return results;
		}

		public async Task<FlowStatus> WriteFlowStatus(FlowStatus flowStatus)
        {
            var point = PointData.Measurement("flowStatus")
                .Tag("flowId", flowStatus.FlowId.ToString())
                .Tag("name", flowStatus.DisplayName)
                .Tag("status", flowStatus.Status)
                .Field("successPercentage", flowStatus.SuccessPercentage)
                .Field("runCount", flowStatus.RunCount)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            await Task.Run(() =>
			{
				_influxDBService.Write(write =>
				{
					write.WritePoint(point, "poc-bucket", "fourtop-ict");
				});
			});

			return flowStatus;
		}
	}
}
