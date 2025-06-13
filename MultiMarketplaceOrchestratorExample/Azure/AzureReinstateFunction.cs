using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Azure
{
    public static class AzureReinstateFunction
    {
        [Function(nameof(AzureReinstate))]
        public static OrchestrationResultModel AzureReinstate([ActivityTrigger] AzureOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to remove the suspended state
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
