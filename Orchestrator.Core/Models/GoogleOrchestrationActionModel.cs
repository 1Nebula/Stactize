using System;
using System.Collections.Generic;
using System.Text;

namespace Orchestrator.Core.Models
{
    /// <summary>
    ///  Orchestration Action Model for Google Cloud Marketplace events
    /// </summary>
    public class GoogleOrchestrationActionModel : OrchestrationActionBaseModel
    {
        /// <summary>
        /// Override for Google Cloud Marketplace orchestration actions
        /// </summary>
        public override Marketplace OriginatingMarketplace => Marketplace.Google;
        /// <summary>
        /// The Google Cloud Marketplace product ID for the subscription/entitlement
        /// </summary>
        public required string ProductId { get; set; }
        /// <summary>
        /// The Google Cloud Marketplace billing plan for the subscription/entitlement
        /// </summary>
        public required string PlanId { get; set; }
        /// <summary>
        /// The Google Cloud Marketplace order ID for the subscription/entitlement
        /// </summary>
        public required string OrderId { get; set; }
    }
}
