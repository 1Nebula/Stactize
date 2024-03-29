using AutoFixture;
using DurableFunctionOrchestratorExample;
using FluentAssertions;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Stactize.DurableFunctionOrchestratorExample.Tests
{
    [TestFixture]
    public class DurableOrchestratorTests
    {
        private Fixture _fixture;
        private DurableFunctionOrchestrator _sut;
        private Mock<IServiceBusService> _serviceBusServiceContext;
        private Mock<ILogger<DurableFunctionOrchestrator>> _loggerContext;
        private Mock<DurableTaskClient> _durableOrchestrationClientContext;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _fixture = new Fixture();

            _serviceBusServiceContext = new Mock<IServiceBusService>();
            _serviceBusServiceContext.Setup(x => x.SendResultToStactize(It.IsAny<OrchestrationResultModel>()))
                                     .Returns(Task.CompletedTask);

            _loggerContext = new Mock<ILogger<DurableFunctionOrchestrator>>();
            _durableOrchestrationClientContext = new Mock<DurableTaskClient>("Test durable task client");

            _sut = new DurableFunctionOrchestrator(_serviceBusServiceContext.Object, _loggerContext.Object);
        }

        [Test]
        public async Task TriggerOrchestrator_Should_StartNewDurableOrchestratorAsync()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();

            //Act
            await _sut.TriggerOrchestrator(knownAction, _durableOrchestrationClientContext.Object);

            //Assert
            _durableOrchestrationClientContext.Verify(x => x.ScheduleNewOrchestrationInstanceAsync(Constants.DurableOrchestrator, knownAction, It.IsAny<StartOrchestrationOptions>(), CancellationToken.None));

        }

        [Test]
        public async Task RunOrchestratorWithEventCreate_Should_CallActivityAsync()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Create)
                                          .Create();

            var knownResultModel = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Create, knownActionModel, It.IsAny<TaskOptions>()))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await _sut.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Create, knownActionModel, It.IsAny<TaskOptions>()));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel, It.IsAny<TaskOptions>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);

        }

        [Test]
        public async Task RunOrchestratorWithEventDelete_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Delete)
                                          .Create();

            var knownResultModel = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Delete, knownActionModel, It.IsAny<TaskOptions>()))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await _sut.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Delete, knownActionModel, It.IsAny<TaskOptions>()));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel, It.IsAny<TaskOptions>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);

        }

        [Test]
        public async Task RunOrchestratorWithEventReinstate_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Reinstate)
                                          .Create();

            var knownResultModel = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Reinstate, knownActionModel, It.IsAny<TaskOptions>()))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await _sut.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Reinstate, knownActionModel, It.IsAny<TaskOptions>()));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel, It.IsAny<TaskOptions>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);
        }

        [Test]
        public async Task RunOrchestratorWithEventSuspend_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Suspend)
                                          .Create();

            var knownResultModel = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Suspend, knownActionModel, It.IsAny<TaskOptions>()))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await _sut.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Suspend, knownActionModel, It.IsAny<TaskOptions>()));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel, It.IsAny<TaskOptions>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);
        }

        [Test]
        public async Task RunOrchestratorWithEventUpdate_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Update)
                                          .Create();

            var knownResultModel = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Update, knownActionModel, It.IsAny<TaskOptions>()))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await _sut.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Update, knownActionModel, It.IsAny<TaskOptions>()));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel, It.IsAny<TaskOptions>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);
        }

        [Test]
        public async Task RunOrchestratorWithEventNull_Should_CompleteOrchestrationWithFail()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, (SubscriptionEvent)999)
                                          .Create();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);

            //Act
            var result = await _sut.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, It.IsAny<OrchestrationResultModel>(), It.IsAny<TaskOptions>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.State.Should().Be(OrchestrationState.Failed);
        }

        [Test]
        public async Task CompleteOrchestratorAction_Should_SendOrchestrationResult()
        {
            //Arrange
            var knownOrchestrationResult = _fixture.Create<OrchestrationResultModel>();

            //Act
            await _sut.CompleteOrchestratorAction(knownOrchestrationResult);

            //Assert
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(knownOrchestrationResult));
        }
    }
}