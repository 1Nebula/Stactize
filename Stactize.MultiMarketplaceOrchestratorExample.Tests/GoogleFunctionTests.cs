using AutoFixture;
using MultiMarketplaceOrchestratorExample.Google;
using Orchestrator.Core;
using Orchestrator.Core.Models;
using Shouldly;

namespace Stactize.MultiMarketplaceOrchestratorExample.Tests
{
    public class GoogleFunctionTests
    {
        private readonly Fixture _fixture;

        public GoogleFunctionTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void GoogleCreate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<GoogleOrchestrationActionModel>();

            //Act
            var result = GoogleCreateFunction.GoogleCreate(action);

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
        public void GoogleUpdate_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<GoogleOrchestrationActionModel>();

            //Act
            var result = GoogleUpdateFunction.GoogleUpdate(action);

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
        public void GoogleDelete_Should_ReturnOrchestrationResultModel()
        {
            //Arrange
            var action = _fixture.Create<GoogleOrchestrationActionModel>();

            //Act
            var result = GoogleDeleteFunction.GoogleDelete(action);

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
