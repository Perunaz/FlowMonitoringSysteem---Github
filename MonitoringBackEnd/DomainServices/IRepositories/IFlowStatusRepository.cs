using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Core.Domain.Flow;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace DomainServices.IRepositories
{
    public interface IFlowStatusRepository
	{
		Task<FlowStatus> WriteFlowStatus(FlowStatus flowStatus);
		Task<IEnumerable<FlowStatusModel>> GetAll();
    }
}