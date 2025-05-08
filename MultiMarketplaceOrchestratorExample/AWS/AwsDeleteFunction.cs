using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.AWS
{
    public static class AwsDeleteFunction
    {
        [Function(nameof(AwsDelete))]
        public static OrchestrationResultModel AwsDelete([ActivityTrigger] AwsOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to remove any instances and user logins
            return orchestrationAction.CreateSuccessResult(new Uri("about:blank"));
        }
    }
}
