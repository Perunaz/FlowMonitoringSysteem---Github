using Coravel.Invocable;
using DomainServices.IRepositories;
using Core.Domain;

namespace Infrastructure.Invocables
{
	public class WriteRandomFlowStatusInvocable : IInvocable
	{
		private readonly IFlowStatusRepository _flowStatusWriter;

		public WriteRandomFlowStatusInvocable(IFlowStatusRepository flowStatusWriter)
		{
			_flowStatusWriter = flowStatusWriter;
		}

		public Task Invoke()
		{
			int randomValue = new Random().Next(0, 5);

			FlowStatus flowStatus = new FlowStatus("1", 5, "test-flow", randomValue);

			_flowStatusWriter.WriteFlowStatus(flowStatus);

			return Task.CompletedTask;
		}
	}
}