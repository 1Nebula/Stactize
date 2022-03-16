using AutoFixture;
using DurableFunctionOrchestratorExample;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using Orchestrator.Core.Constants;
using System.Threading.Tasks;

namespace Stactize.DurableFunctionOrchestratorExample.Tests
{
    [TestFixture]
    public class DurableOrchestratorTests
    {
        private Fixture fixture;
        private DurableFunctionOrchestrator SUT;
        private Mock<IServiceBusService> _serviceBusServiceContext;
        private Mock<ILogger> _loggerContext;
        private Mock<IDurableOrchestrationClient> _durableOrchestrationClientContext;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            fixture = new Fixture();

            _serviceBusServiceContext = new Mock<IServiceBusService>();
            _serviceBusServiceContext.Setup(x => x.SendResultToStactize(It.IsAny<OrchestrationResultModel>()))
                                     .Returns(Task.CompletedTask);

            _loggerContext = new Mock<ILogger>();
            _durableOrchestrationClientContext = new Mock<IDurableOrchestrationClient>();

            SUT = new DurableFunctionOrchestrator(_serviceBusServiceContext.Object);
        }

        [Test]
        public async Task TriggerOrchestrator_Should_StartNewDurableOrchestratorAsync()
        {
            //Arrange
            var knownAction = fixture.Create<OrchestrationActionModel>();

            //Act
            await DurableFunctionOrchestrator.TriggerOrchestrator(knownAction, _loggerContext.Object, _durableOrchestrationClientContext.Object);

            //Assert
            _durableOrchestrationClientContext.Verify(x => x.StartNewAsync(Constants.DurableOrchestrator, null, knownAction));

        }

        [Test]
        public async Task RunOrchestratorWithEventCreate_Should_CallActivityAsync()
        {
            //Arrange
            var knownActionModel = fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Create)
                                          .Create();

            var knownResultModel = fixture.Create<OrchestrationResultModel>();

            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Create, knownActionModel))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await SUT.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Create, knownActionModel));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);

        }

        [Test]
        public async Task RunOrchestratorWithEventDelete_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Delete)
                                          .Create();

            var knownResultModel = fixture.Create<OrchestrationResultModel>();

            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Delete, knownActionModel))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await SUT.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Delete, knownActionModel));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);

        }

        [Test]
        public async Task RunOrchestratorWithEventReinstate_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Reinstate)
                                          .Create();

            var knownResultModel = fixture.Create<OrchestrationResultModel>();

            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Reinstate, knownActionModel))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await SUT.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Reinstate, knownActionModel));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);

        }

        [Test]
        public async Task RunOrchestratorWithEventSuspend_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Suspend)
                                          .Create();

            var knownResultModel = fixture.Create<OrchestrationResultModel>();

            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Suspend, knownActionModel))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await SUT.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Suspend, knownActionModel));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);
        }

        [Test]
        public async Task RunOrchestratorWithEventUpdate_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Update)
                                          .Create();

            var knownResultModel = fixture.Create<OrchestrationResultModel>();

            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Update, knownActionModel))
                   .ReturnsAsync(knownResultModel);

            //Act
            var result = await SUT.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(Constants.DurableActivity.Update, knownActionModel));
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, knownResultModel));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.Should().BeEquivalentTo(knownResultModel);
        }

        [Test]
        public async Task RunOrchestratorWithEventNull_Should_CompleteOrchestrationWithFail()
        {
            //Arrange
            var knownActionModel = fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, (SubscriptionEvent)999)
                                          .Create();

            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionModel>())
                   .Returns(knownActionModel);

            //Act
            var result = await SUT.RunOrchestrator(context.Object);

            //Assert
            context.Verify(x => x.CallActivityAsync(Constants.DurableActivity.CompleteAction, It.IsAny<OrchestrationResultModel>()));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OrchestrationResultModel));
            result.State.Should().Be(OrchestrationState.Failed);
        }

        [Test]
        public async Task CompleteOrchestratorAction_Should_SendOrchestrationResult()
        {
            //Arrange
            var knownOrchestrationResult = fixture.Create<OrchestrationResultModel>();

            //Act
            await SUT.CompleteOrchestratorAction(knownOrchestrationResult, _loggerContext.Object);

            //Assert
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(knownOrchestrationResult));
        }
    }
}