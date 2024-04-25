using DomainServices.Services.Interface;
using DomainServices.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Flow;

namespace DomainServices.Services.Implementation
{
    public class FlowStatusService : IFlowStatusService
	{
		private readonly IFlowStatusRepository _flowStatusRepository;

		public FlowStatusService(IFlowStatusRepository flowStatusRepository) 
		{
			_flowStatusRepository = flowStatusRepository;
		}

		public Task CreateFlowStatus(FlowStatus flowStatus)
		{
			return _flowStatusRepository.WriteFlowStatus(flowStatus);
		}

		public async Task<IEnumerable<FlowStatusModel>> GetAll()
		{
			return await _flowStatusRepository.GetAll();
		}

		public FlowStatus GetFlow(string id)
		{
			throw new NotImplementedException();

		}
    }
}
