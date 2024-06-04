using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using Infrastructure.IServices;

namespace Infrastructure.Services
{
    public class InfluxDBService : IInfluxDBService
    {
        private readonly string _token;
        private readonly string _url;
        private static InfluxDBClient _influxDBClient;
        private static readonly object _lock = new object();

        public InfluxDBService()
        {
            _token = Environment.GetEnvironmentVariable("INFLUXDB_TOKEN");
            _url = Environment.GetEnvironmentVariable("INFLUXDB_URL");

            // Ensure singleton instance is created
            EnsureClientInstance();
        }

        private void EnsureClientInstance()
        {
            if (_influxDBClient == null)
            {
                lock (_lock)
                {
                    if (_influxDBClient == null)
                    {
                        _influxDBClient = new InfluxDBClient(_url, _token);
                    }
                }
            }
        }

        public void Write(Action<WriteApi> action)
        {
            var write = _influxDBClient.GetWriteApi();
            action(write);
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
        {
            var query = _influxDBClient.GetQueryApi();
            return await action(query);
        }
    }
}