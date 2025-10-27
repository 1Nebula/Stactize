using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Stripe
{
    public static class StripeUpdateFunction
    {
        [Function(nameof(StripeUpdate))]
        public static OrchestrationResultModel StripeUpdate([ActivityTrigger] StripeOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to update any instances and send potentially new url
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
