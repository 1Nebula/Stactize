using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Google
{
    public static class GoogleUpdateFunction
    {
        [Function(nameof(GoogleUpdate))]
        public static OrchestrationResultModel GoogleUpdate([ActivityTrigger] GoogleOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to update any instances and send potentially new url
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
