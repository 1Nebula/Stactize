using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Azure
{
    public static class AzureSuspendFunction
    {
        [Function(nameof(AzureSuspend))]
        public static OrchestrationResultModel AzureSuspend([ActivityTrigger] AzureOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to put instances into a suspended state
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
