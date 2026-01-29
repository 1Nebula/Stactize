using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Stripe
{
    public static class StripeReinstateFunction
    {
        [Function(nameof(StripeReinstate))]
        public static OrchestrationResultModel StripeReinstate([ActivityTrigger] StripeOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to remove the suspended state
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
