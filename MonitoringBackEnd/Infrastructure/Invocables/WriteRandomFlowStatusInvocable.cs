using Coravel.Invocable;
using DomainServices.IRepositories;
using Core.Domain;

namespace Infrastructure.Invocables
{
	public class WriteRandomFlowStatusInvocable : IInvocable
	{
		private readonly IFlowStatusRepository _flowStatusRepository;

		public WriteRandomFlowStatusInvocable(IFlowStatusRepository flowStatusRepository)
		{
            _flowStatusRepository = flowStatusRepository;
		}

		public Task Invoke()
		{
			int randomValue = new Random().Next(0, 5);

			FlowStatus flowStatus = new FlowStatus("1", 5, "test-flow", randomValue);

            return _flowStatusRepository.WriteFlowStatus(flowStatus);
		}
	}
}