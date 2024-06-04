using Core.Domain.Flow;
using DomainServices.Services.Interface;
using EventBusAPI.Controllers;
using EventBusAPI.RabbitMQ.IProducers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests
{
    public class FlowStatusControllerTests
    {
        private readonly Mock<IFlowStatusService> _mockFlowStatusService;
        private readonly Mock<IRabbitMQProducer> _mockRabbitMQProducer;
        private readonly FlowStatusController _controller;

        public FlowStatusControllerTests()
        {
            _mockFlowStatusService = new Mock<IFlowStatusService>();
            _mockRabbitMQProducer = new Mock<IRabbitMQProducer>();
            _controller = new FlowStatusController(_mockFlowStatusService.Object, _mockRabbitMQProducer.Object);
        }

        [Fact]
        public async Task AddFlowStatus_ShouldCreateFlowStatusAndSendMessage_WhenStatusChangesFromSucceededToFailed()
        {
            // Arrange
            var flowId = Guid.NewGuid();
            var succeededStatus = new FlowStatus(flowId, "Test Flow", "Succeeded", 100, 10);
            var failedStatus = new FlowStatus(flowId, "Test Flow", "Failed", 0, 11);

            _mockFlowStatusService.SetupSequence(s => s.CreateFlowStatus(It.IsAny<FlowStatus>()))
                .ReturnsAsync(succeededStatus)
                .ReturnsAsync(failedStatus);

            _controller.AddFlowStatus(succeededStatus);

            // Act
            var result = _controller.AddFlowStatus(failedStatus);

            // Assert
            _mockFlowStatusService.Verify(s => s.CreateFlowStatus(It.IsAny<FlowStatus>()), Times.Exactly(2));
            _mockRabbitMQProducer.Verify(p => p.Connect("flowstatus_changed"), Times.Once);
            _mockRabbitMQProducer.Verify(p => p.SendMessage(failedStatus, "flowstatus_changed"), Times.Once);
            _mockRabbitMQProducer.Verify(p => p.Disconnect(), Times.Once);
            Assert.Equal(failedStatus, result);
        }

        [Fact]
        public async Task GetAllFlowStatuses_ShouldReturnAllFlowStatuses()
        {
            // Arrange
            var flowStatusList = new List<FlowStatusModel>
            {
                new FlowStatusModel(Guid.NewGuid(), "Flow 1", "Succeeded", 100, 5, "2024-05-24T12:00:00"),
                new FlowStatusModel(Guid.NewGuid(), "Flow 2", "Failed", 0, 10, "2024-05-24T12:10:00")
            };

            _mockFlowStatusService.Setup(s => s.GetAll()).ReturnsAsync(flowStatusList);

            // Act
            var result = await _controller.GetAllFlowStatuses();

            // Assert
            Assert.Equal(flowStatusList, result);
            _mockFlowStatusService.Verify(s => s.GetAll(), Times.Once);
        }
    }
}