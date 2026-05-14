using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Google
{
    public static class GoogleCreateFunction
    {
        [Function(nameof(GoogleCreate))]
        public static OrchestrationResultModel GoogleCreate([ActivityTrigger] GoogleOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
