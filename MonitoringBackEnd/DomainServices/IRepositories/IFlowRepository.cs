using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;

namespace DomainServices.IRepositories
{
	internal interface IFlowRepository
	{
		Flow GetFlowById(string flowId);
		Task<int> AddFlow(Flow newFlow);
		IEnumerable<Flow> GetAllFlows();
	}
}
