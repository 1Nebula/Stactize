using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.AWS
{
    public static class AwsUpdateFunction
    {
        [Function(nameof(AwsUpdate))]
        public static OrchestrationResultModel AwsUpdate([ActivityTrigger] AwsOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to update any instances and send potentially new url
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
