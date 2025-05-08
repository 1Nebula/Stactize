using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using MultiMarketplaceOrchestratorExample.AWS;
using MultiMarketplaceOrchestratorExample.Azure;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using System.Text.Json;

namespace MultiMarketplaceOrchestratorExample
{
    public class MultiMarketplaceOrchestrator
    {
        private readonly ILogger<MultiMarketplaceOrchestrator> _logger;
        private readonly IServiceBusService _serviceBusService;

        public MultiMarketplaceOrchestrator(ILogger<MultiMarketplaceOrchestrator> logger, IServiceBusService serviceBusService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceBusService = serviceBusService ?? throw new ArgumentNullException(nameof(serviceBusService));
        }

        [Function(nameof(IngressQueueOrchestratorTrigger))]
        public async Task IngressQueueOrchestratorTrigger(
            [ServiceBusTrigger("%ingressQueueName%", Connection = "ingressServiceBusConnectionString")] OrchestrationActionBaseModel myQueueItem,
            [DurableClient] DurableTaskClient durableOrchestrationClient)
        {
            _logger.LogInformation("C# ServiceBus queue trigger function processed message: {QueueMessage}", JsonSerializer.Serialize(myQueueItem));

            //Start a new orchestrator - send service bus message as input data
            var instanceId = await durableOrchestrationClient.ScheduleNewOrchestrationInstanceAsync(nameof(RunOrchestrator), myQueueItem);

            //Log the instance Id - this Id can be used to track the results of the orchestration run
            _logger.LogInformation("Orchestrator instance created with Id: {InstanceId}", instanceId);
        }

        [Function(nameof(RunOrchestrator))]
        public async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            //Retrieve message from trigger to consume
            var orchestratorAction = context.GetInput<OrchestrationActionBaseModel>();

            OrchestrationResultModel result;
            try
            {
                //Determine the action to take and generate a result
                var activity = (orchestratorAction.OriginatingMarketplace, orchestratorAction.Event) switch
                {
                    (Marketplace.Azure, SubscriptionEvent.Create) => nameof(AzureCreateFunction.AzureCreate),
                    (Marketplace.Azure,SubscriptionEvent.Update) => nameof(AzureUpdateFunction.AzureUpdate),
                    (Marketplace.Azure,SubscriptionEvent.Suspend) => nameof(AzureSuspendFunction.AzureSuspend),
                    (Marketplace.Azure, SubscriptionEvent.Reinstate) => nameof(AzureReinstateFunction.AzureReinstate),
                    (Marketplace.Azure,SubscriptionEvent.Delete) => nameof(AzureDeleteFunction.AzureDelete),
                    (Marketplace.Aws, SubscriptionEvent.Create) => nameof(AwsCreateFunction.AwsCreate),
                    (Marketplace.Aws, SubscriptionEvent.Update) => nameof(AwsUpdateFunction.AwsUpdate),
                    (Marketplace.Aws, SubscriptionEvent.Delete) => nameof(AwsDeleteFunction.AwsDelete),
                    _ => throw new NotImplementedException("No handler for orchestration event.")
                };

                //Call the correct activity based on the orchestration event
                result = await context.CallActivityAsync<OrchestrationResultModel>(activity, orchestratorAction);
            }
            catch (Exception ex)
            {
                //Catch any exceptions so that a response can be sent back to Stactize.
                //This ensures the system is up to date and relevant client/administrator emails are sent
                result = orchestratorAction.CreateFailedResult(new FailureMessageModel
                {
                    FriendlyMessage = $"Oops! Something went wrong while trying to action against subscription with Id {orchestratorAction.SubscriptionId}",
                    DetailedMessage = ex.Message
                });
            }

            //Call the CompleteOrchestratorAction activity to send the response back to Stactize
            await context.CallActivityAsync(nameof(CompleteOrchestratorAction), result);
        }

        ///<summary>
        /// The Complete Orchestrator activity is responsible for returning the result of the orchestration back to Stactize.
        /// This will trigger any relevant emails to be sent to the user and and required updates to be sent to the Microsoft Fulfilment API.
        /// </summary>
        [Function(nameof(CompleteOrchestratorAction))]
        public async Task CompleteOrchestratorAction([ActivityTrigger] OrchestrationResultModel orchestrationResult)
        {
            _logger.LogInformation("Completing orchestration action for subscription with Id {SubscriptionId}", orchestrationResult.SubscriptionId);

            await _serviceBusService.SendResultToStactize(orchestrationResult);

        }
    }
}
