using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.Google
{
    public static class GoogleDeleteFunction
    {
        [Function(nameof(GoogleDelete))]
        public static OrchestrationResultModel GoogleDelete([ActivityTrigger] GoogleOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to remove any instances and user logins
            return orchestrationAction.CreateSuccessResult(new Uri("about:blank"));
        }
    }
}
