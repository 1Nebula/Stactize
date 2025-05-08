using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using MultiMarketplaceOrchestratorExample;
using MultiMarketplaceOrchestratorExample.AWS;
using MultiMarketplaceOrchestratorExample.Azure;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using Shouldly;

namespace Stactize.MultiMarketplaceOrchestratorExample.Tests
{
    public class MultiMarketplaceOrchestratorTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<MultiMarketplaceOrchestrator>> _loggerContext;
        private readonly Mock<IServiceBusService> _serviceBusServiceContext;
        private readonly Mock<DurableTaskClient> _durableOrchestrationClientContext;
        private readonly MultiMarketplaceOrchestrator _sut;

        public MultiMarketplaceOrchestratorTests()
        {
            _fixture = new Fixture();

            _loggerContext = new Mock<ILogger<MultiMarketplaceOrchestrator>>();
            _serviceBusServiceContext = new Mock<IServiceBusService>();
            _durableOrchestrationClientContext = new Mock<DurableTaskClient>("Test durable task client");

            _sut = new MultiMarketplaceOrchestrator(_loggerContext.Object, _serviceBusServiceContext.Object);
        }

        [TestCase(typeof(AzureOrchestrationActionModel))]
        [TestCase(typeof(AwsOrchestrationActionModel))]
        public async Task IngressQueueOrchestratorTrigger_Should_ScheduleNewOrchestrationInstance(Type actionType)
        {
            //Arrange
            var orchestrationActionModel = new SpecimenContext(_fixture).Resolve(actionType) as OrchestrationActionBaseModel;

            //Act
            var act = () => _sut.IngressQueueOrchestratorTrigger(orchestrationActionModel!, _durableOrchestrationClientContext.Object);

            //Assert
            await act.ShouldNotThrowAsync();
            _durableOrchestrationClientContext.Verify(x => x.ScheduleNewOrchestrationInstanceAsync(
                    nameof(MultiMarketplaceOrchestrator.RunOrchestrator),
                    orchestrationActionModel,
                    It.IsAny<StartOrchestrationOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestCase(SubscriptionEvent.Create, nameof(AzureCreateFunction.AzureCreate))]
        [TestCase(SubscriptionEvent.Update, nameof(AzureUpdateFunction.AzureUpdate))]
        [TestCase(SubscriptionEvent.Delete, nameof(AzureDeleteFunction.AzureDelete))]
        [TestCase(SubscriptionEvent.Reinstate, nameof(AzureReinstateFunction.AzureReinstate))]
        [TestCase(SubscriptionEvent.Suspend, nameof(AzureSuspendFunction.AzureSuspend))]
        public async Task RunOrchestrator_WithAzureOrchestrationActionModel_Should_CallCompleteOrchestrator(SubscriptionEvent subscriptionEvent, string taskName)
        {
            //Arrange
            var orchestrationActionModel = _fixture.Build<AzureOrchestrationActionModel>()
                .With(x => x.Event, subscriptionEvent)
                .Create();
            var expectedResult = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionBaseModel>()).Returns(orchestrationActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(taskName, orchestrationActionModel, It.IsAny<TaskOptions>()))
                .ReturnsAsync(expectedResult);

            //Act
            var act = () => _sut.RunOrchestrator(context.Object);

            //Assert
            await act.ShouldNotThrowAsync();
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(taskName, orchestrationActionModel, It.IsAny<TaskOptions>()),
                Times.Once);
            context.Verify(x => x.CallActivityAsync(nameof(MultiMarketplaceOrchestrator.CompleteOrchestratorAction), expectedResult, It.IsAny<TaskOptions>()),
                Times.Once);
        }

        [TestCase(SubscriptionEvent.Create, nameof(AwsCreateFunction.AwsCreate))]
        [TestCase(SubscriptionEvent.Update, nameof(AwsUpdateFunction.AwsUpdate))]
        [TestCase(SubscriptionEvent.Delete, nameof(AwsDeleteFunction.AwsDelete))]
        public async Task RunOrchestrator_WithAwsOrchestrationActionModel_Should_CallCompleteOrchestrator(SubscriptionEvent subscriptionEvent, string taskName)
        {
            //Arrange
            var orchestrationActionModel = _fixture.Build<AwsOrchestrationActionModel>()
                .With(x => x.Event, subscriptionEvent)
                .Create();
            var expectedResult = _fixture.Create<OrchestrationResultModel>();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionBaseModel>()).Returns(orchestrationActionModel);
            context.Setup(x => x.CallActivityAsync<OrchestrationResultModel>(taskName, orchestrationActionModel, It.IsAny<TaskOptions>()))
                .ReturnsAsync(expectedResult);

            //Act
            var act = () => _sut.RunOrchestrator(context.Object);

            //Assert
            await act.ShouldNotThrowAsync();
            context.Verify(x => x.CallActivityAsync<OrchestrationResultModel>(taskName, orchestrationActionModel, It.IsAny<TaskOptions>()),
                Times.Once);
            context.Verify(x => x.CallActivityAsync(nameof(MultiMarketplaceOrchestrator.CompleteOrchestratorAction), expectedResult, It.IsAny<TaskOptions>()),
                Times.Once);
        }

        [TestCase]
        public async Task RunOrchestrator_WithInvalid_DoesNotThrow()
        {
            //Arrange
            var orchestrationActionModel = _fixture.Build<AwsOrchestrationActionModel>()
                .With(x => x.Event, SubscriptionEvent.Suspend)
                .Create();

            var context = new Mock<TaskOrchestrationContext>();
            context.Setup(x => x.GetInput<OrchestrationActionBaseModel>()).Returns(orchestrationActionModel);

            //Act
            var act = () => _sut.RunOrchestrator(context.Object);

            //Assert
            await act.ShouldNotThrowAsync();
            context.Verify(x => x.CallActivityAsync(
                nameof(MultiMarketplaceOrchestrator.CompleteOrchestratorAction),
                It.Is<OrchestrationResultModel>(m => m.State == OrchestrationState.Failed),
                It.IsAny<TaskOptions>()),
               Times.Once);
        }

        [Test]
        public async Task CompleteOrchestratorAction_Should_SendResultToStactize()
        {
            //Arrange
            var result = _fixture.Create<OrchestrationResultModel>();


            //Act
            var act = () => _sut.CompleteOrchestratorAction(result);

            //Assert
            await act.ShouldNotThrowAsync();
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(result), Times.Once);
        }
    }
}