using Core.Domain;

namespace DomainServices.Services.Interface
{
	internal interface IFlowService
	{
		public Task<int> CreateFlow(Flow flow);
	}
}
