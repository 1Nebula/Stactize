
using AutoFixture;
using MultiMarketplaceOrchestratorExample.AWS;
using Orchestrator.Core;
using Orchestrator.Core.Models;
using Shouldly;

namespace Stactize.MultiMarketplaceOrchestratorExample.Tests
{
    public class AwsFunctionTests
    {
        private readonly Fixture _fixture;

        public AwsFunctionTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void AwsCreate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AwsOrchestrationActionModel>();

            //Act
            var result = AwsCreateFunction.AwsCreate(action);

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
        public void AwsUpdate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AwsOrchestrationActionModel>();

            //Act
            var result = AwsUpdateFunction.AwsUpdate(action);

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
        public void AwsDelete_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<AwsOrchestrationActionModel>();

            //Act
            var result = AwsDeleteFunction.AwsDelete(action);

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
