using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestrator.Core;
using Orchestrator.Core.Constants;
using Orchestrator.Core.Contracts;
using Orchestrator.Core.Models;
using System.Text.Json;

namespace StandardFunctionOrchestratorExample
{
    public class OrchestratorFunction
    {
        private readonly ILogger<OrchestratorFunction> _logger;
        private readonly IServiceBusService _serviceBusService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public OrchestratorFunction(
            ILogger<OrchestratorFunction> logger,
            IServiceBusService serviceBusService,
            IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceBusService = serviceBusService ?? throw new ArgumentNullException(nameof(serviceBusService));
            _jsonSerializerOptions = jsonSerializerOptions?.Value ?? throw new ArgumentException(nameof(jsonSerializerOptions));
        }

        [Function(nameof(OrchestratorFunction))]
        public async Task Run(
            [ServiceBusTrigger(ConfigurationConstants.ingressQueueName_, Connection = ConfigurationConstants.ingressConnectionString)]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);

            try
            {
                var orchestratorAction = message.Body.ToObjectFromJson<OrchestrationActionModel>(_jsonSerializerOptions);

                //Determine the action to take and generate a result
                var orchestrationResult = orchestratorAction.Event switch
                {
                    SubscriptionEvent.Create => CreateInfrastructure(orchestratorAction),
                    SubscriptionEvent.Update => Update(orchestratorAction),
                    SubscriptionEvent.Suspend => Suspend(orchestratorAction),
                    SubscriptionEvent.Delete => DeleteInfrastructure(orchestratorAction),
                    SubscriptionEvent.Reinstate => Reinstate(orchestratorAction),
                    _ => throw new NotImplementedException("No handler for orchestration event.")
                };

                await _serviceBusService.SendResultToStactize(orchestrationResult);

                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
            catch (JsonException ex)
            {
                // Catch this exception here and do not complete it or throw to prevent any retries.
                // This type of error must be fixed before attempting to process the message again.
                _logger.LogCritical(ex, "An error occurred while deserializing the Stactize Orchestration Action." +
                    "Error: {errorMessage}. Service bus message: {message}", ex.Message, message.Body.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the Stactize Orchestration Action." +
                    "Error: {errorMessage}. Service bus message: {message}", ex.Message, message.Body.ToString());

                var orchestratorAction = message.Body.ToObjectFromJson<OrchestrationActionModel>();
                var errorResult = orchestratorAction.CreateFailedResult(new FailureMessageModel
                {
                    FriendlyMessage = "An error occurred while orchestrating the action.",
                    DetailedMessage = ex.ToString()
                });
                await _serviceBusService.SendResultToStactize(errorResult);

                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
        }

        ///<summary>
        /// The Create activity is responsible for creating all the infrastructure and any set-up steps required for a new subscription.
        /// After the resources have been created and all the necessary registrations have been completed, Stactize expects a URL to be returned.
        /// This will be sent to the user in an email and can be either a unique URL or a first-time login url. 
        /// </summary>
        public OrchestrationResultModel CreateInfrastructure(OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Delete activity is responsible for tearing down any infrastructure and removing the references to a user.
        /// Microsoft recommends you keep the data of the user for 30 days after they have cancelled their subscription.
        /// This event will be received either after a user cancels their subscription or after 30 days of non-payment
        /// </summary>
        public OrchestrationResultModel DeleteInfrastructure(OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to delete your instances
            return orchestrationAction.CreateSuccessResult(null);
        }

        ///<summary>
        /// The Reinstate activity is responsible for re-activating a subscription after payment resumes for a suspended subscription. 
        /// </summary>
        public OrchestrationResultModel Reinstate(OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Suspend activity is responsible for suspending a subscription after not paying. 
        /// This can be as simple as setting flag in your application or decommissioning the app entirely.
        /// </summary>
        public OrchestrationResultModel Suspend(OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }

        ///<summary>
        /// The Update activity is responsible for updating a subscription to a different plan or to change the number of seats for a subscription.
        /// The plan to change to will be in the PlanId field of the orchestrationAction.
        /// If the URL changes, it must be returned in the LoginUrl field of the Orchestration Result Model.
        /// </summary>
        public OrchestrationResultModel Update(OrchestrationActionModel orchestrationAction)
        {
            //Call your orchestration functions here to create your instances and generate a url for a user to log in to
            return orchestrationAction.CreateSuccessResult(new Uri("http://mycoolwebsite.net"));
        }
    }
}
