using AutoFixture;
using MultiMarketplaceOrchestratorExample.Stripe;
using Orchestrator.Core;
using Orchestrator.Core.Models;
using Shouldly;

namespace Stactize.MultiMarketplaceOrchestratorExample.Tests
{
    public class StripeFunctionTests
    {
        private readonly Fixture _fixture;

        public StripeFunctionTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void AzureCreate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<StripeOrchestrationActionModel>();

            //Act
            var result = StripeCreateFunction.StripeCreate(action);

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
            var action = _fixture.Create<StripeOrchestrationActionModel>();

            //Act
            var result = StripeUpdateFunction.StripeUpdate(action);

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
            var action = _fixture.Create<StripeOrchestrationActionModel>();

            //Act
            var result = StripeSuspendFunction.StripeSuspend(action);

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
            var action = _fixture.Create<StripeOrchestrationActionModel>();

            //Act
            var result = StripeReinstateFunction.StripeReinstate(action);

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
            var action = _fixture.Create<StripeOrchestrationActionModel>();

            //Act
            var result = StripeDeleteFunction.StripeDelete(action);

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
