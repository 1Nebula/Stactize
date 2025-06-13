using System;
using System.Collections.Generic;

namespace Orchestrator.Core.Models
{
    public abstract class OrchestrationActionBaseModel
    {
        /// <summary>
        /// The marketplace that the orchestration event originates from - must be overridden in all derived classes
        /// </summary>
        public abstract Marketplace OriginatingMarketplace { get; }

        /// <summary>
        /// Uniquely identifies the operation
        /// </summary>
        public required string OperationId { get; set; }

        /// <summary>
        /// Identifies the application the message was intended for. Used for queue name
        /// </summary>
        public required string ApplicationId { get; set; }

        /// <summary>
        /// The Id of the tenant that initiated the action
        /// </summary>
        public required string TenantId { get; set; }

        /// <summary>
        /// The Id of the subscription that initiated the action
        /// </summary>
        public required string SubscriptionId { get; set; }

        /// <summary>
        /// The type of action the orchestrator needs to undertake
        /// </summary>
        public required SubscriptionEvent Event { get; set; }

        /// <summary>
        /// List of values resulting from the signup form
        /// </summary>
        public IEnumerable<UserFieldValueModel> UserFields { get; set; } = [];

        /// <summary>
        /// Dictionary of extra details associated with the action
        /// </summary>
        public IDictionary<string, string>? ActionDetails { get; set; }

        /// <summary>
        /// Creates an OrchestrationResultModel with State set to Succeeded
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="firstTimeLoginUrl">Optional</param>
        /// <returns></returns>
        public OrchestrationResultModel CreateSuccessResult(Uri loginUrl, Uri? firstTimeLoginUrl = null)
        {
            return new OrchestrationResultModel()
            {
                ApplicationId = this.ApplicationId,
                OperationId = this.OperationId,
                SubscriptionId = this.SubscriptionId,
                TenantId = this.TenantId,
                State = OrchestrationState.Succeeded,
                LoginUrl = loginUrl,
                FirstTimeLoginUrl = firstTimeLoginUrl
            };
        }

        /// <summary>
        /// Creates an OrchestrationResultModel with State set to Failed
        /// </summary>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        public OrchestrationResultModel CreateFailedResult(FailureMessageModel failureMessage)
        {
            return new OrchestrationResultModel()
            {
                ApplicationId = this.ApplicationId,
                OperationId = this.OperationId,
                SubscriptionId = this.SubscriptionId,
                TenantId = this.TenantId,
                State = OrchestrationState.Failed,
                FailureMessage = failureMessage,
                LoginUrl = new Uri("about:blank")
            };
        }
    }

    /// <summary>
    /// Describes different subscription events 
    /// </summary>
    public enum SubscriptionEvent
    {
        Create,
        Update,
        Suspend,
        Delete,
        Reinstate
    }

    /// <summary>
    /// Describes the originating marketplace
    /// </summary>
    public enum Marketplace
    {
        Azure,
        Aws
    }
}
