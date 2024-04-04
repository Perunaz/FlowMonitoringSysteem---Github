using Core.Domain;
using DomainServices.IRepositories;
using DomainServices.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services.Implementation
{
	internal class FlowService : IFlowService
	{
		private readonly IFlowRepository _flowRepository;

		public FlowService(IFlowRepository flowRepository) 
		{
			_flowRepository = flowRepository;
		}

		public async Task<int> CreateFlow(Flow flow)
		{
			return await _flowRepository.AddFlow(flow);
		}
	}
}
