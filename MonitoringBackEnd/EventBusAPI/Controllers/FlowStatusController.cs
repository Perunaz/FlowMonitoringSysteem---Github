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
        private static readonly Dictionary<Guid, string> _flowStatusDictionary = new Dictionary<Guid, string>();


        public FlowStatusController(IFlowStatusService flowStatusService, IRabbitMQProducer rabbitMQProducer)
		{
			_flowStatusService = flowStatusService;
			_rabbitMQProducer = rabbitMQProducer;
        }

		public FlowStatus AddFlowStatus(FlowStatus flowStatus)
		{
			_flowStatusService.CreateFlowStatus(flowStatus);

            if (_flowStatusDictionary.TryGetValue(flowStatus.FlowId, out string currentStatus))
            {
                // Check if the status has changed from "Succeeded" to "Failed"
                if (currentStatus == "Succeeded" && flowStatus.Status == "Failed")
                {
                    // Send the message to the queue
                    _rabbitMQProducer.Connect("flowstatus_changed");
                    _rabbitMQProducer.SendMessage(flowStatus, "flowstatus_changed");
                    _rabbitMQProducer.Disconnect();
                }
            }

            // Update the flow status in the dictionary
            _flowStatusDictionary[flowStatus.FlowId] = flowStatus.Status;

            return flowStatus;
		}

        public async Task<IEnumerable<FlowStatusModel>> GetAllFlowStatuses()
        {
			return await _flowStatusService.GetAll();
        }
    }
}
