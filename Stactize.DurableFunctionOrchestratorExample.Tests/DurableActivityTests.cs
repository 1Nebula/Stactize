using AutoFixture;
using DurableFunctionOrchestratorExample;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using System.Threading.Tasks;

namespace Stactize.DurableFunctionOrchestratorExample.Tests
{
    [TestFixture]
    public class DurableActivityTests
    {
        private Fixture fixture;
        private DurableFunctionOrchestrator SUT;
        private Mock<IServiceBusService> _serviceBusServiceContext;
        private Mock<ILogger> _loggerContext;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            fixture = new Fixture();

            _serviceBusServiceContext = new Mock<IServiceBusService>();
            _serviceBusServiceContext.Setup(x => x.SendResultToStactize(It.IsAny<OrchestrationResultModel>()))
                                     .Returns(Task.CompletedTask);

            _loggerContext = new Mock<ILogger>();

            SUT = new DurableFunctionOrchestrator(_serviceBusServiceContext.Object);
        }

        [Test]
        public void CreateInfrastructure_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = fixture.Create<OrchestrationActionModel>();

            //Act
            var result = SUT.CreateInfrastructure(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void Update_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = fixture.Create<OrchestrationActionModel>();

            //Act
            var result = SUT.Update(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void Reinstate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = fixture.Create<OrchestrationActionModel>();

            //Act
            var result = SUT.Reinstate(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void Suspend_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = fixture.Create<OrchestrationActionModel>();

            //Act
            var result = SUT.Suspend(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }

        [Test]
        public void DeleteInfrastructure_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var knownAction = fixture.Create<OrchestrationActionModel>();

            //Act
            var result = SUT.DeleteInfrastructure(knownAction, _loggerContext.Object);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
        }
    }
}