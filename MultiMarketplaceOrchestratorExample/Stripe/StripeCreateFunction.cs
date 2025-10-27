using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Stripe
{
    public static class StripeCreateFunction
    {
        [Function(nameof(StripeCreate))]
        public static OrchestrationResultModel StripeCreate([ActivityTrigger] StripeOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
