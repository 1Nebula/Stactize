using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Azure
{
    public static class AzureUpdateFunction
    {
        [Function(nameof(AzureUpdate))]
        public static OrchestrationResultModel AzureUpdate([ActivityTrigger] AzureOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to update any instances and send potentially new url
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
