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
                var flux = "from(bucket:\"poc-bucket\") |> range(start: 0)";
                var tables = await query.QueryAsync(flux, "fourtop-ict");
                return tables.SelectMany(table =>
                table.Records.Select(record =>
                    {
                        var flowStatus = new FlowStatusModel(
                            record.GetValueByKey("flow").ToString(),
							int.Parse(record.GetValue().ToString()), 
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
						.Field("value", flowStatus.StateCode)
                        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

					write.WritePoint(point, "poc-bucket", "fourtop-ict");
				});
			});

			return flowStatus;
		}
	}
}
