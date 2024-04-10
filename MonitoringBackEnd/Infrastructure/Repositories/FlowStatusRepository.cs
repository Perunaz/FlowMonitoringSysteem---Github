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

		public async Task<FlowStatus> WriteFlowStatus(FlowStatus flowStatus)
		{
			await Task.Run(() =>
			{
				_influxDBService.Write(write =>
				{
					var point = PointData.Measurement("flowStatus")
						.Tag("flow", flowStatus.FlowName)
						.Field("value", flowStatus.StateCode)
						.Timestamp(DateTime.UtcNow, WritePrecision.Ns);

					write.WritePoint(point, "poc-bucket", "fourtop-ict");
				});
			});

			return flowStatus;
		}
	}
}
