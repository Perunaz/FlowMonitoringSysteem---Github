using InfluxDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IServices
{
    public interface IInfluxDBService
    {
        void Write(Action<WriteApi> action);
        Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action);
    }
}
