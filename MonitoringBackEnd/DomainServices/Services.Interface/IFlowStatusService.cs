using Core.Domain;

namespace DomainServices.Services.Interface
{
	public interface IFlowStatusService
	{
		public FlowStatus GetFlow(string id);
		public Task<FlowStatus> CreateFlowStatus(FlowStatus flowStatus);
		public IEnumerable<FlowStatus> GetAll();
	}
}
