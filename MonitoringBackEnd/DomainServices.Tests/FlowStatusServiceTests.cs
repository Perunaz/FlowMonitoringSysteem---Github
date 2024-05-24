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
            FlowStatus testFlowStatus = new FlowStatus();
            flowStatusRepoMock.Setup(f => f.WriteFlowStatus(testFlowStatus)).ReturnsAsync(testFlowStatus);

            // Act
            FlowStatus result = await sut.CreateFlowStatus(testFlowStatus);

            // Assert
            Assert.IsType<FlowStatus>(result);
            Assert.Equal(testFlowStatus, result);
        }
    }
}
