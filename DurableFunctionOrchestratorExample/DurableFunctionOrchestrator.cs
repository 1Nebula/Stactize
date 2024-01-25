using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Orchestrator.Core;
using Orchestrator.Core.Constants;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;

namespace DurableFunctionOrchestratorExample
{
    public class DurableFunctionOrchestrator
    {
        private readonly IServiceBusService _serviceBusService;
        private readonly ILogger<DurableFunctionOrchestrator> _logger;

        public DurableFunctionOrchestrator(IServiceBusService serviceBusService,
            ILogger<DurableFunctionOrchestrator> logger)
        {
            _serviceBusService = serviceBusService ?? throw new ArgumentNullException(nameof(serviceBusService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function(Constants.OrchestrationTrigger)]
        public async Task TriggerOrchestrator(
            [ServiceBusTrigger(ConfigurationConstants.ingressQueueName_, Connection = ConfigurationConstants.ingressConnectionString)] OrchestrationActionModel myQueueItem,
            [DurableClient] DurableTaskClient durableOrchestrationClient)
        {
            _logger.LogInformation("C# ServiceBus queue trigger function processed message: {queueMessage}", JsonSerializer.Serialize(myQueueItem));

            //Start a new orchestrator - send service bus message as input data
            var instanceId = await durableOrchestrationClient.ScheduleNewOrchestrationInstanceAsync(Constants.DurableOrchestrator, myQueueItem);

            //Log the instance Id - this Id can be used to track the results of the orchestration run
            _logger.LogInformation("Orchestrator instance created with Id: {instanceId}", instanceId);
        }

        [Function(Constants.DurableOrchestrator)]
        public async Task<OrchestrationResultModel> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            //Retrieve message from trigger to consume
            var orchestratorAction = context.GetInput<OrchestrationActionModel>();

            OrchestrationResultModel result;
            try
            {
                //Determine the action to take and generate a result
                var activity = orchestratorAction.Event switch
                {
                    SubscriptionEvent.Create => Constants.DurableActivity.Create,
                    SubscriptionEvent.Update => Constants.DurableActivity.Update,
                    SubscriptionEvent.Suspend => Constants.DurableActivity.Suspend,
                    SubscriptionEvent.Delete => Constants.DurableActivity.Delete,
                    SubscriptionEvent.Reinstate => Constants.DurableActivity.Reinstate,
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

            //Call the CompleteAction activity to send the response back to Stactize
            await context.CallActivityAsync(Constants.DurableActivity.CompleteAction, result);

            //Return result for logging and information purposes
            return result;
        }


        ///<summary>
        /// The Create activity is responsible for creating all the infrastructure and any set-up steps required for a new subscription.
        /// After the resources have been created and all the necessary registrations have been completed, Stactize expects a URL to be returned.
        /// This will be sent to the user in an email and can be either a unique URL or a first-time login url. 
        /// </summary>
        [Function(Constants.DurableActivity.Create)]
        public OrchestrationResultModel CreateInfrastructure([ActivityTrigger] OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(null);
        }

        ///<summary>
        /// The Delete activity is responsible for tearing down any infrastructure and removing the references to a user.
        /// Microsoft recommends you keep the data of the user for 30 days after they have cancelled their subscription.
        /// This event will be received either after a user cancels their subscription or after 30 days of non-payment
        /// </summary>
        [Function(Constants.DurableActivity.Delete)]
        public OrchestrationResultModel DeleteInfrastructure([ActivityTrigger] OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to delete your instances
            return orchestrationAction.CreateSuccessResult(null);
        }

        ///<summary>
        /// The Reinstate activity is responsible for re-activating a subscription after payment resumes for a suspended subscription. 
        /// </summary>
        [Function(Constants.DurableActivity.Reinstate)]
        public OrchestrationResultModel Reinstate([ActivityTrigger] OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Suspend activity is responsible for suspending a subscription after not paying. 
        /// This can be as simple as setting flag in your application or decommissioning the app entirely.
        /// </summary>
        [Function(Constants.DurableActivity.Suspend)]
        public OrchestrationResultModel Suspend([ActivityTrigger] OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Update activity is responsible for updating a subscription to a different plan or to change the number of seats for a subscription.
        /// The plan to change to will be in the PlanId field of the orchestrationAction.
        /// If the URL changes, it must be returned in the LoginUrl field of the Orchestration Result Model.
        /// </summary>
        [Function(Constants.DurableActivity.Update)]
        public OrchestrationResultModel Update([ActivityTrigger] OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Complete Orchestrator activity is responsible for returning the result of the orchestration back to Stactize.
        /// This will trigger any relevant emails to be sent to the user and and required updates to be sent to the Microsoft Fulfilment API.
        /// </summary>
        [Function(Constants.DurableActivity.CompleteAction)]
        public async Task CompleteOrchestratorAction([ActivityTrigger] OrchestrationResultModel orchestrationResult)
        {
            _logger.LogInformation("Completing orchestration action for subscription with Id {subscriptionId}", orchestrationResult.SubscriptionId);

            await _serviceBusService.SendResultToStactize(orchestrationResult);

        }

    }
}
