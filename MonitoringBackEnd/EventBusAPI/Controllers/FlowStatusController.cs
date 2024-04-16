using Core.Domain;
using DomainServices.Services.Implementation;
using DomainServices.Services.Interface;
using EventBusAPI.RabbitMQ.IProducers;
using EventBusAPI.RabbitMQ.Producers;
using Microsoft.AspNetCore.Mvc;

namespace EventBusAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FlowStatusController: ControllerBase
	{
		private readonly IFlowStatusService _flowStatusService;
		private readonly IRabbitMQProducer _rabbitMQProducer;

		public FlowStatusController(IFlowStatusService flowStatusService, IRabbitMQProducer rabbitMQProducer)
		{
			_flowStatusService = flowStatusService;
			_rabbitMQProducer = rabbitMQProducer;
		}

		[HttpPost("addFlowStatus")]
		public FlowStatus AddFlowStatus(FlowStatus flowStatus)
		{
			_flowStatusService.CreateFlowStatus(flowStatus);
			//send the inserted product data to the queue and consumer will listening this data from queue
			_rabbitMQProducer.PublishEvent(flowStatus, "flowstatus", "flowstatus_created", "");
			return flowStatus;
		}

        [HttpGet("getAllFlowStatuses")]
        public async Task<IEnumerable<(FlowStatus, string)>> GetAllFlowStatuses()
        {
			return await _flowStatusService.GetAll();
        }
    }
}
