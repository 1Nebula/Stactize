using System;
using System.ComponentModel.DataAnnotations;

namespace Orchestrator.Core
{
    public class OrchestrationResultModel
    {
        /// <summary>
        /// Id of the operation that was executed
        /// </summary>
        [Required]
        public string OperationId { get; set; }

        /// <summary>
        /// Id of the Tenant that the operation was executed for
        /// </summary>
        [Required]
        public string TenantId { get; set; }

        /// <summary>
        /// Id of the Subscription the operation was executed for
        /// </summary>
        [Required]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Id of the Application the operation was executed for
        /// </summary>
        [Required]
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
        /// The URL that will be sent to the user when creation is successful.
        /// If FirstTimeLoginUrl is set, this Url will be used when navigating to the app via the SaaS portal.
        /// Note that this URL is always required, even if your application is configured to send emails without login links.
        /// </summary>
        public Uri LoginUrl { get; set; }

        /// <summary>
        /// Optional URL that will be sent to the user once activation is successful instead of LoginUrl.
        /// Use URLs with one-time pins or specific path for users to activate their account
        /// </summary>
        public Uri FirstTimeLoginUrl { get; set; }

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
