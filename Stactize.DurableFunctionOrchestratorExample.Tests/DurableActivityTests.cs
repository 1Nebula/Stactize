using AutoFixture;
using DurableFunctionOrchestratorExample;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using System.Threading.Tasks;

namespace Stactize.DurableFunctionOrchestratorExample.Tests
{
    [TestFixture]
    public class DurableActivityTests
    {
        private Fixture _fixture;
        private DurableFunctionOrchestrator _sut;
        private Mock<IServiceBusService> _serviceBusServiceContext;
        private Mock<ILogger> _loggerContext;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _fixture = new Fixture();

            _serviceBusServiceContext = new Mock<IServiceBusService>();
            _serviceBusServiceContext.Setup(x => x.SendResultToStactize(It.IsAny<OrchestrationResultModel>()))
                                     .Returns(Task.CompletedTask);

            _loggerContext = new Mock<ILogger>();

            _sut = new DurableFunctionOrchestrator(_serviceBusServiceContext.Object);
        }

        [Test]
        public void CreateInfrastructure_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();

            //Act
            var result = _sut.CreateInfrastructure(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void Update_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();

            //Act
            var result = _sut.Update(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void Reinstate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();

            //Act
            var result = _sut.Reinstate(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void Suspend_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();

            //Act
            var result = _sut.Suspend(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void DeleteInfrastructure_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();

            //Act
            var result = _sut.DeleteInfrastructure(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }
    }
}