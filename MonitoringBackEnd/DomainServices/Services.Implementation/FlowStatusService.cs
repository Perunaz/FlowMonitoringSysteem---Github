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

        public async Task<FlowStatus> CreateFlowStatus(FlowStatus flowStatus)
        {
            await _flowStatusRepository.WriteFlowStatus(flowStatus);
            return flowStatus;
        }

        public async Task<IEnumerable<FlowStatusModel>> GetAll()
		{
			return await _flowStatusRepository.GetAll();
		}
    }
}
