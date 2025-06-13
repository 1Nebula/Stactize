using Microsoft.Azure.Functions.Worker;
using Orchestrator.Core;
using Orchestrator.Core.Models;

namespace MultiMarketplaceOrchestratorExample.AWS
{
    public static class AwsCreateFunction
    {
        [Function(nameof(AwsCreate))]
        public static OrchestrationResultModel AwsCreate([ActivityTrigger] AwsOrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("https://mywebsite.com"));
        }
    }
}
