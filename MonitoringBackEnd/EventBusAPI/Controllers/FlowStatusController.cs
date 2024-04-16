using Core.Domain.Flow;
using DomainServices.Services.Implementation;
using DomainServices.Services.Interface;
using EventBusAPI.RabbitMQ.IProducers;
using EventBusAPI.RabbitMQ.Producers;
using Microsoft.AspNetCore.Mvc;

namespace EventBusAPI.Controllers
{
	public class FlowStatusController: ControllerBase
	{
		private readonly IFlowStatusService _flowStatusService;
		private readonly IRabbitMQProducer _rabbitMQProducer;

		public FlowStatusController(IFlowStatusService flowStatusService, IRabbitMQProducer rabbitMQProducer)
		{
			_flowStatusService = flowStatusService;
			_rabbitMQProducer = rabbitMQProducer;
		}

		public FlowStatus AddFlowStatus(FlowStatus flowStatus)
		{
			_flowStatusService.CreateFlowStatus(flowStatus);

			//send the inserted product data to the queue and consumer will listening this data from queue
			_rabbitMQProducer.Connect("flowstatus_created");
			_rabbitMQProducer.SendMessage(flowStatus, "flowstatus_created");
			_rabbitMQProducer.Disconnect();

			return flowStatus;
		}

        public async Task<IEnumerable<FlowStatusModel>> GetAllFlowStatuses()
        {
			return await _flowStatusService.GetAll();
        }
    }
}
