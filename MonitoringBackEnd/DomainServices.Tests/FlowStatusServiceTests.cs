using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Flow;
using DomainServices.IRepositories;
using DomainServices.Services.Implementation;
using Moq;

namespace DomainServices.Tests
{
    public class FlowStatusServiceTests
    {
        private readonly Mock<IFlowStatusRepository> flowStatusRepoMock;

        private readonly FlowStatusService sut;

        public FlowStatusServiceTests()
        {
            flowStatusRepoMock = new();

            sut = new FlowStatusService(flowStatusRepoMock.Object);
        }

        [Fact]
        public async Task Should_Create_FlowStatus_And_Return_FlowStatus_Object()
        {
            // Arrange
            FlowStatus testFlowStatus = new FlowStatus(new Guid(), "Status1", "Succeeded", 80, 10);
            FlowStatus testFlowStatus2 = new FlowStatus("18d645d2-67fb-4b4a-b29b-d60c8c13057c", "Status2", "Failed", 60, 15);
            flowStatusRepoMock.Setup(f => f.WriteFlowStatus(testFlowStatus)).ReturnsAsync(testFlowStatus);
            flowStatusRepoMock.Setup(f => f.WriteFlowStatus(testFlowStatus2)).ReturnsAsync(testFlowStatus2);

            // Act
            FlowStatus result = await sut.CreateFlowStatus(testFlowStatus);
            FlowStatus result2 = await sut.CreateFlowStatus(testFlowStatus2);

            // Assert
            Assert.IsType<FlowStatus>(result);
            Assert.IsType<FlowStatus>(result2);
            Assert.Equal(testFlowStatus, result);
            Assert.Equal(testFlowStatus2, result2);
        }

        [Fact]
        public async Task Should_GetAll_FlowStatus_And_Return_FlowStatus_Array_Object()
        {
            // Arrange
            var expectedFlowStatuses = new List<FlowStatusModel>
            {
                new FlowStatusModel(new Guid(), "Status1", "Succeeded", 80, 10, "2024-05-24T08:00:00Z"),
                new FlowStatusModel("18d645d2-67fb-4b4a-b29b-d60c8c13057c", "Status2", "Failed", 60, 15, "2024-05-23T08:00:00Z")
            };
            flowStatusRepoMock.Setup(f => f.GetAll()).ReturnsAsync(expectedFlowStatuses);

            // Act
            var result = await sut.GetAll();

            // Assert
            Assert.IsType<FlowStatusModel[]>(result.ToArray());
            Assert.Equal(expectedFlowStatuses.Count, result.Count());
            Assert.Equal(expectedFlowStatuses, result);
        }

        [Fact]
        public async Task Should_Get_Overridden_ToString_FlowStatus()
        {
            //Arrange
            var flowStatusModel = new FlowStatus("18d645d2-67fb-4b4a-b29b-d60c8c13057c", "Status2", "Failed", 60, 15);

            //Act
            var result = flowStatusModel.ToString();

            //Assert
            Assert.Equal("FlowId: 18d645d2-67fb-4b4a-b29b-d60c8c13057c, DisplayName: Status2, Status: Failed, SuccessPercentage: 60, RunCount: 15", result);
        }

        [Fact]
        public async Task Should_Get_Overridden_ToString_FlowStatusModel()
        {
            //Arrange
            var flowStatus = new FlowStatusModel("18d645d2-67fb-4b4a-b29b-d60c8c13057c", "Status2", "Failed", 60, 15, "2024-05-23T08:00:00Z");

            //Act
            var result = flowStatus.ToString();

            //Assert
            Assert.Equal("FlowId: 18d645d2-67fb-4b4a-b29b-d60c8c13057c, DisplayName: Status2, Status: Failed, SuccessPercentage: 60, RunCount: 15, Time: 2024-05-23T08:00:00Z", result);
        }
    }
}
