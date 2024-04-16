using Core.Domain;
using DomainServices.Services.Interface;
using DomainServices.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public Task<IEnumerable<(FlowStatus, string)>> GetAll()
		{
			return _flowStatusRepository.GetAll();
		}

		public FlowStatus GetFlow(string id)
		{
			throw new NotImplementedException();

		}
    }
}
