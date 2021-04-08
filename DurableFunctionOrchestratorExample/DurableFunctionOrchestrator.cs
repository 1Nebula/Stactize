using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;

namespace DurableFunctionOrchestratorExample
{
    public class DurableFunctionOrchestrator
    {
        private readonly IServiceBusService _serviceBusService;

        public DurableFunctionOrchestrator(IServiceBusService serviceBusService)
        {
            _serviceBusService = serviceBusService ?? throw new ArgumentNullException(nameof(serviceBusService));
        }

        [FunctionName(Constants.OrchestrationTrigger)]
        public static async Task TriggerOrchestrator(
            [ServiceBusTrigger(Constants.QueueName, Connection = Constants.ingressConnectionString)]OrchestrationActionModel myQueueItem,
            ILogger log,
            [DurableClient]IDurableOrchestrationClient durableOrchestrationClient)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            //Start a new orchestrator - send service bus message as input data
            var instanceId = await durableOrchestrationClient.StartNewAsync(Constants.DurableOrchestrator, null, myQueueItem);

            //Log the instance Id - this Id can be used to track the results of the orchestration run
            log.LogInformation($"Orchestrator instance created with Id: {instanceId}");
        }

        [FunctionName(Constants.DurableOrchestrator)]
        public async Task<OrchestrationResultModel> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            //Retrieve message from trigger to consume
            var orchestratorAction = context.GetInput<OrchestrationActionModel>();

            OrchestrationResultModel result = null;

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
                //This ensures the system is up to date and relevent client/administrator emails are sent
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
        /// The Create activity is responsible for creating all the infrastructure and any setup steps required for a new subscription.
        /// After the resources have been created and all the necesseary registrations have been completed, Stactize expects a URL to be returned.
        /// This will be sent to the user in an email and can be either a unique URL or a first-time login url. 
        /// </summary>
        [FunctionName(Constants.DurableActivity.Create)]
        public OrchestrationResultModel CreateInfrastructure([ActivityTrigger] OrchestrationActionModel orchestrationAction, ILogger log)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(null);
        }

        ///<summary>
        /// The Delete activity is responsible for tearing down any infrastructure and removing the references to a user.
        /// Microsoft recommends you keep the data of the user for 30 days after they have cancelled their subscription.
        /// This event will be recieved either after a user cancels their subscription or after 30 days of non-payment
        /// </summary>
        [FunctionName(Constants.DurableActivity.Delete)]
        public OrchestrationResultModel DeleteInfrastructure([ActivityTrigger] OrchestrationActionModel orchestrationAction, ILogger log)
        {
            //Call your orchestration functions here to delete your instances
            return orchestrationAction.CreateSuccessResult(null);
        }

        ///<summary>
        /// The Reinstate activity is responsible for re-activating a subscription after payment resumes for a suspended subscription. 
        /// </summary>
        [FunctionName(Constants.DurableActivity.Reinstate)]
        public OrchestrationResultModel Reinstate([ActivityTrigger] OrchestrationActionModel orchestrationAction, ILogger log)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Suspend activity is resonsible for suspending a subscription after not paying. 
        /// This can be as simple as setting flag in your application or decomissioning the app entirely.
        /// </summary>
        [FunctionName(Constants.DurableActivity.Suspend)]
        public OrchestrationResultModel Suspend([ActivityTrigger] OrchestrationActionModel orchestrationAction, ILogger log)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Update activity is responsible for updating a subscription to a different plan or to change the number of seats for a subscription.
        /// The plan to change to will be in the PlanId field of the orchestrationAction.
        /// If the URL changes, it must be returned in the LoginUrl field of the Orchestration Result Model.
        /// </summary>
        [FunctionName(Constants.DurableActivity.Update)]
        public OrchestrationResultModel Update([ActivityTrigger] OrchestrationActionModel orchestrationAction, ILogger log)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Complete Orchestrator activity is responsible for returning the result of the orchestration back to Stactize.
        /// This will trigger any relevant emails to be sent to the user and and required updates to be sent to the Microsoft fulfillement API.
        /// </summary>
        [FunctionName(Constants.DurableActivity.CompleteAction)]
        public async Task CompleteOrchestratorAction([ActivityTrigger] OrchestrationResultModel orchestrationResult, ILogger log)
        {
            log.LogInformation($"Completing orchestration action for subscription with Id {orchestrationResult.SubscriptionId}");

            await _serviceBusService.SendResultToStactize(orchestrationResult);

        }

    }
}
