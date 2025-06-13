using AutoFixture;
using MultiMarketplaceOrchestratorExample.Azure;
using Orchestrator.Core;
using Orchestrator.Core.Models;
using Shouldly;

namespace Stactize.MultiMarketplaceOrchestratorExample.Tests
{
    public class AzureFunctionTests
    {
        private readonly Fixture _fixture;

        public AzureFunctionTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void AzureCreate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AzureOrchestrationActionModel>();

            //Act
            var result = AzureCreateFunction.AzureCreate(action);

            //Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldNotBeNull(),
                () => result.ShouldBeOfType<OrchestrationResultModel>(),
                () => result.OperationId.ShouldBe(action.OperationId),
                () => result.ApplicationId.ShouldBe(action.ApplicationId),
                () => result.SubscriptionId.ShouldBe(action.SubscriptionId),
                () => result.TenantId.ShouldBe(action.TenantId),
                () => result.State.ShouldBe(OrchestrationState.Succeeded)
            );
        }

        [Test]
        public void AzureUpdate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AzureOrchestrationActionModel>();

            //Act
            var result = AzureUpdateFunction.AzureUpdate(action);

            //Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldNotBeNull(),
                () => result.ShouldBeOfType<OrchestrationResultModel>(),
                () => result.ApplicationId.ShouldBe(action.ApplicationId),
                () => result.OperationId.ShouldBe(action.OperationId),
                () => result.SubscriptionId.ShouldBe(action.SubscriptionId),
                () => result.TenantId.ShouldBe(action.TenantId),
                () => result.State.ShouldBe(OrchestrationState.Succeeded)
            );
        }

        [Test]
        public void AzureSuspend_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AzureOrchestrationActionModel>();

            //Act
            var result = AzureSuspendFunction.AzureSuspend(action);

            //Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldNotBeNull(),
                () => result.ShouldBeOfType<OrchestrationResultModel>(),
                () => result.ApplicationId.ShouldBe(action.ApplicationId),
                () => result.OperationId.ShouldBe(action.OperationId),
                () => result.SubscriptionId.ShouldBe(action.SubscriptionId),
                () => result.TenantId.ShouldBe(action.TenantId),
                () => result.State.ShouldBe(OrchestrationState.Succeeded)
            );
        }

        [Test]
        public void AzureReinstate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AzureOrchestrationActionModel>();

            //Act
            var result = AzureReinstateFunction.AzureReinstate(action);

            //Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldNotBeNull(),
                () => result.ShouldBeOfType<OrchestrationResultModel>(),
                () => result.ApplicationId.ShouldBe(action.ApplicationId),
                () => result.OperationId.ShouldBe(action.OperationId),
                () => result.SubscriptionId.ShouldBe(action.SubscriptionId),
                () => result.TenantId.ShouldBe(action.TenantId),
                () => result.State.ShouldBe(OrchestrationState.Succeeded)
            );
        }

        [Test]
        public void AzureDelete_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AzureOrchestrationActionModel>();

            //Act
            var result = AzureDeleteFunction.AzureDelete(action);

            //Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldNotBeNull(),
                () => result.ShouldBeOfType<OrchestrationResultModel>(),
                () => result.ApplicationId.ShouldBe(action.ApplicationId),
                () => result.OperationId.ShouldBe(action.OperationId),
                () => result.SubscriptionId.ShouldBe(action.SubscriptionId),
                () => result.TenantId.ShouldBe(action.TenantId),
                () => result.State.ShouldBe(OrchestrationState.Succeeded)
            );
        }
    }
}
