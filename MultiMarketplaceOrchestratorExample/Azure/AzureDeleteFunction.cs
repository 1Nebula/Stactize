using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Azure
{
    public static class AzureDeleteFunction
    {
        [Function(nameof(AzureDelete))]
        public static OrchestrationResultModel AzureDelete([ActivityTrigger] AzureOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to remove any instances and user logins
            return orchestrationAction.CreateSuccessResult(new Uri("about:blank"));
        }
    }
}
