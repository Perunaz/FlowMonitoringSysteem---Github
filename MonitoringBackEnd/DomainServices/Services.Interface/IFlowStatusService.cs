using Core.Domain.Flow;
using DomainServices.IRepositories;

namespace DomainServices.Services.Interface
{
    public interface IFlowStatusService
	{
        public Task<IEnumerable<FlowStatusModel>> GetAll();
        public Task<FlowStatus> CreateFlowStatus(FlowStatus flowStatus);
	}
}
