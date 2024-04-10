using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services

{
	public class InfluxDBService
	{
		private readonly string _token;
		private readonly string _url;

		public InfluxDBService(IConfiguration configuration)
		{
			_token = configuration.GetValue<string>("influxdb:Token");
			_url = configuration.GetValue<string>("influxdb:Url");
		}

		public void Write(Action<WriteApi> action)
		{
			using var client = new InfluxDBClient(_url, _token);
			using var write = client.GetWriteApi();
			action(write);
		}

		public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
		{
			using var client = new InfluxDBClient(_url, _token);
			var query = client.GetQueryApi();
			return await action(query);
		}
	}
}