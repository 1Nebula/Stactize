using System;

namespace Orchestrator.Core
{
    public class OrchestrationResultModel
    {
        /// <summary>
        /// Id of the operation that was executed
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// Id of the Tenant that the operation was executed for
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Id of the Subscription the operation was executed for
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Id of the Application the operation was executed for
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Indicated whether the orchestration was successful or not
        /// </summary>
        public OrchestrationState State { get; set; }

        /// <summary>
        /// Describes error details. Only read if State = Failed. Can be left null
        /// </summary>
        public FailureMessageModel FailureMessage { get; set; }

        /// <summary>
        /// The URL that will be sent to the user when creation is successful
        /// </summary>
        public Uri LoginUrl { get; set; }
    }

    public class FailureMessageModel
    {
        /// <summary>
        /// A friendly message that will be sent to the user
        /// </summary>
        public string FriendlyMessage { get; set; }

        /// <summary>
        /// A detailed message that will be sent to the SupportEmail (as configured in Application configuration)
        /// </summary>
        public string DetailedMessage { get; set; }
    }

    public enum OrchestrationState
    {
        Failed,
        Succeeded
    }
}
