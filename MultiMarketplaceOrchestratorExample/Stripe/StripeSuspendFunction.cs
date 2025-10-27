using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Stripe
{
    public static class StripeSuspendFunction
    {
        [Function(nameof(StripeSuspend))]
        public static OrchestrationResultModel StripeSuspend([ActivityTrigger] StripeOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to put instances into a suspended state
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
