using Core.Domain.Flow;

namespace DomainServices.Services.Interface
{
    public interface IFlowStatusService
	{
		public FlowStatus GetFlow(string id);
		public Task<FlowStatus> CreateFlowStatus(FlowStatus flowStatus);
		public Task<IEnumerable<FlowStatusModel>> GetAll();
	}
}
