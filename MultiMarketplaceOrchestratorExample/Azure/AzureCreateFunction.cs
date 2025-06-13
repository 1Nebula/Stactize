using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Azure
{

    public static class AzureCreateFunction
    {
        [Function(nameof(AzureCreate))]
        public static OrchestrationResultModel AzureCreate([ActivityTrigger] AzureOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }

    }
}
