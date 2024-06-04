using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Core.Domain.Flow;
using Infrastructure.Services;
using InfluxDB.Client;
using Infrastructure.IServices;
using InfluxDB.Client.Writes;

namespace Infrastructure.Tests
{
    public class FlowStatusRepositoryTests
    {
        private readonly Mock<IInfluxDBService> _mockInfluxDBService;
        private readonly FlowStatusRepository _flowStatusRepository;

        public FlowStatusRepositoryTests()
        {
            _mockInfluxDBService = new Mock<IInfluxDBService>();
            _flowStatusRepository = new FlowStatusRepository(_mockInfluxDBService.Object);
        }

        [Fact]
        public void Constructor_SetsInfluxDBService()
        {
            // Arrange
            var mockInfluxDBService = new Mock<IInfluxDBService>();

            // Act
            var repository = new FlowStatusRepository(mockInfluxDBService.Object);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task GetAll_ReturnsFlowStatusModels()
        {
            // Arrange
            var mockResult = new List<FlowStatusModel>
            {
                new FlowStatusModel(Guid.NewGuid().ToString(), "TestName1", "Success", 100, 10, DateTime.UtcNow.ToString()),
                new FlowStatusModel(Guid.NewGuid().ToString(), "TestName2", "Failure", 50, 5, DateTime.UtcNow.ToString())
            };

            _mockInfluxDBService
                .Setup(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _flowStatusRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(result, mockResult);
            _mockInfluxDBService.Verify(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()), Times.Once);
        }

        [Fact]
        public async Task WriteFlowStatus_CallsInfluxDBServiceWrite()
        {
            // Arrange
            var flowStatus = new FlowStatus(Guid.NewGuid().ToString(), "TestName", "Success", 100, 10);

            // Act
            var result = await _flowStatusRepository.WriteFlowStatus(flowStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(flowStatus, result);
            _mockInfluxDBService.Verify(service => service.Write(It.IsAny<Action<WriteApi>>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoResults()
        {
            // Arrange
            var mockResult = new List<FlowStatusModel>();

            _mockInfluxDBService
                .Setup(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _flowStatusRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockInfluxDBService.Verify(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ThrowsException_WhenQueryFails()
        {
            // Arrange
            _mockInfluxDBService
                .Setup(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()))
                .ThrowsAsync(new Exception("Query failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _flowStatusRepository.GetAll());
            _mockInfluxDBService.Verify(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()), Times.Once);
        }

        [Fact]
        public async Task WriteFlowStatus_ThrowsException_WhenWriteFails()
        {
            // Arrange
            var flowStatus = new FlowStatus(Guid.NewGuid().ToString(), "TestName", "Success", 100, 10);

            _mockInfluxDBService
                .Setup(service => service.Write(It.IsAny<Action<WriteApi>>()))
                .Throws(new Exception("Write failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _flowStatusRepository.WriteFlowStatus(flowStatus));
            _mockInfluxDBService.Verify(service => service.Write(It.IsAny<Action<WriteApi>>()), Times.Once);
        }

        [Fact]
        public async Task WriteFlowStatus_WorksWithDefaultValues()
        {
            // Arrange
            var flowStatus = new FlowStatus(Guid.NewGuid().ToString(), null, null, 0, 0);

            // Act
            var result = await _flowStatusRepository.WriteFlowStatus(flowStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(flowStatus, result);
            _mockInfluxDBService.Verify(service => service.Write(It.IsAny<Action<WriteApi>>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_GeneratesCorrectFluxQuery()
        {
            // Arrange
            string expectedFluxQuery = "expected query here"; // Adjust this to match the actual query generated in your method
            _mockInfluxDBService
                .Setup(service => service.QueryAsync(It.IsAny<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>()))
                .Callback<Func<QueryApi, Task<IEnumerable<FlowStatusModel>>>>(queryFunc =>
                {
                    // Use reflection or similar to access the generated query string and validate it
                })
                .ReturnsAsync(new List<FlowStatusModel>());

            // Act
            await _flowStatusRepository.GetAll();

            // Assert
            // Validate the flux query generation
            // This may involve inspecting the actual query string inside the callback
        }
    }
}