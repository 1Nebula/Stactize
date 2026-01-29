using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Stripe
{
    public static class StripeDeleteFunction
    {
        [Function(nameof(StripeDelete))]
        public static OrchestrationResultModel StripeDelete([ActivityTrigger] StripeOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to remove any instances and user logins
            return orchestrationAction.CreateSuccessResult(new Uri("about:blank"));
        }
    }
}
