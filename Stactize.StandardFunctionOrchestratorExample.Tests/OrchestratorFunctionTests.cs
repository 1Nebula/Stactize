using AutoFixture;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using StandardFunctionOrchestratorExample;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stactize.StandardFunctionOrchestratorExample.Tests
{
    public class OrchestratorFunctionTests
    {
        private Fixture _fixture;
        private JsonSerializerOptions _defaultJsonSerializerOptions;
        private Mock<ILogger<OrchestratorFunction>> _loggerContext;
        private Mock<IServiceBusService> _serviceBusServiceContext;
        private Mock<IOptions<JsonSerializerOptions>> _jsonSerializerOptionsContext;
        private Mock<ServiceBusMessageActions> _messageActionsContext;
        private OrchestratorFunction _sut;

        [OneTimeSetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _loggerContext = new Mock<ILogger<OrchestratorFunction>>();
            _serviceBusServiceContext = new Mock<IServiceBusService>();
            _serviceBusServiceContext.Setup(x => x.SendResultToStactize(It.IsAny<OrchestrationResultModel>()))
                                     .Returns(Task.CompletedTask);
            _jsonSerializerOptionsContext = new Mock<IOptions<JsonSerializerOptions>>();
            _defaultJsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            _defaultJsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            _jsonSerializerOptionsContext.Setup(x => x.Value).Returns(_defaultJsonSerializerOptions);

            _messageActionsContext = new Mock<ServiceBusMessageActions>();

            _sut = new OrchestratorFunction(_loggerContext.Object, _serviceBusServiceContext.Object, _jsonSerializerOptionsContext.Object);
        }

        [Test]
        public async Task TriggerOrchestrator_Should_StartNewDurableOrchestratorAsync()
        {
            //Arrange
            var knownAction = _fixture.Create<OrchestrationActionModel>();
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownAction, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RunOrchestratorWithEventCreate_Should_CallActivityAsync()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Create)
                                          .Create();
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownActionModel, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(It.Is<OrchestrationResultModel>(result =>
                result.State == OrchestrationState.Succeeded &&
                result.ApplicationId == knownActionModel.ApplicationId &&
                result.SubscriptionId == knownActionModel.SubscriptionId &&
                result.TenantId == knownActionModel.TenantId &&
                result.OperationId == knownActionModel.OperationId &&
                result.LoginUrl != null
            )), Times.Once);

        }

        [Test]
        public async Task RunOrchestratorWithEventDelete_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Delete)
                                          .Create();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownActionModel, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(It.Is<OrchestrationResultModel>(result =>
                result.State == OrchestrationState.Succeeded &&
                result.ApplicationId == knownActionModel.ApplicationId &&
                result.SubscriptionId == knownActionModel.SubscriptionId &&
                result.TenantId == knownActionModel.TenantId &&
                result.OperationId == knownActionModel.OperationId
            )), Times.Once);

        }

        [Test]
        public async Task RunOrchestratorWithEventReinstate_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Reinstate)
                                          .Create();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownActionModel, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(It.Is<OrchestrationResultModel>(result =>
                result.State == OrchestrationState.Succeeded &&
                result.ApplicationId == knownActionModel.ApplicationId &&
                result.SubscriptionId == knownActionModel.SubscriptionId &&
                result.TenantId == knownActionModel.TenantId &&
                result.OperationId == knownActionModel.OperationId
            )), Times.Once);
        }

        [Test]
        public async Task RunOrchestratorWithEventSuspend_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Suspend)
                                          .Create();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownActionModel, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(It.Is<OrchestrationResultModel>(result =>
                result.State == OrchestrationState.Succeeded &&
                result.ApplicationId == knownActionModel.ApplicationId &&
                result.SubscriptionId == knownActionModel.SubscriptionId &&
                result.TenantId == knownActionModel.TenantId &&
                result.OperationId == knownActionModel.OperationId
            )), Times.Once);
        }

        [Test]
        public async Task RunOrchestratorWithEventUpdate_Should_CallActivity()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, SubscriptionEvent.Update)
                                          .Create();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownActionModel, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(It.Is<OrchestrationResultModel>(result =>
                result.State == OrchestrationState.Succeeded &&
                result.ApplicationId == knownActionModel.ApplicationId &&
                result.SubscriptionId == knownActionModel.SubscriptionId &&
                result.TenantId == knownActionModel.TenantId &&
                result.OperationId == knownActionModel.OperationId
            )), Times.Once);
        }

        [Test]
        public async Task RunOrchestratorWithEventNull_Should_CompleteOrchestrationWithFail()
        {
            //Arrange
            var knownActionModel = _fixture.Build<OrchestrationActionModel>()
                                          .With(x => x.Event, (SubscriptionEvent)999)
                                          .Create();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(JsonSerializer.Serialize(knownActionModel, _defaultJsonSerializerOptions)));

            //Act
            var act = _sut.Awaiting(x => x.Run(message, _messageActionsContext.Object));

            //Assert
            await act.Should().NotThrowAsync();
            _messageActionsContext.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            _serviceBusServiceContext.Verify(x => x.SendResultToStactize(It.Is<OrchestrationResultModel>(result =>
                result.State == OrchestrationState.Failed &&
                result.ApplicationId == knownActionModel.ApplicationId &&
                result.SubscriptionId == knownActionModel.SubscriptionId &&
                result.TenantId == knownActionModel.TenantId &&
                result.OperationId == knownActionModel.OperationId
            )), Times.Once);
        }
    }
}