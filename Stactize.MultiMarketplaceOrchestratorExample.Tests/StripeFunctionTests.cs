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
        public void StripeCreate_Should_ReturnOrchestrationResultModel()
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
        public void StripeUpdate_Should_ReturnOrchestrationResultModel()
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
        public void StripeSuspend_Should_ReturnOrchestrationResultModel()
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
        public void StripeReinstate_Should_ReturnOrchestrationResultModel()
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
        public void StripeDelete_Should_ReturnOrchestrationResultModel()
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
