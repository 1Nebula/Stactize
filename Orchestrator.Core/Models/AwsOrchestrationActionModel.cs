using System;
using System.Collections.Generic;

namespace Orchestrator.Core.Models
{
    /// <summary>
    /// Orchestration Action Model for AWS events
    /// </summary>
    public class AwsOrchestrationActionModel : OrchestrationActionBaseModel
    {
        /// <summary>
        /// Override for AWS marketplace orchestration actions
        /// </summary>
        public override Marketplace OriginatingMarketplace => Marketplace.Aws;

        /// <summary>
        /// The latest entitlements linked to the subscription that the orchestration action is for
        /// </summary>
        public IEnumerable<AwsEntitlementModel> Entitlements { get; set; } = [];

        /// <summary>
        /// The product code linked to the subscription that initiated the action
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// The offer identifier for the orchestration action. Populated if the relevant subscription is linked to a 
        /// private offer.
        /// </summary>
        public string? OfferIdentifier { get; set; }
    }

    /// <summary>
    /// An entitlement describes what the subscription has access to. 
    /// These are configured in the AWS Marketplace console and are selected by the user when subscribing.
    /// </summary>
    public class AwsEntitlementModel
    {
        /// <summary>
        /// The Id of the customers AWS Account
        /// </summary>
        public required string AwsAccountId { get; set; }
        /// <summary>
        /// An obfuscated version of <see cref="AwsAccountId"/>. Will be deprecated on 31 March 2026.
        /// </summary>
        public required string CustomerIdentifier { get; set; }
        /// <summary>
        /// The name of the dimension
        /// </summary>
        public required string Dimension { get; set; }
        /// <summary>
        /// The date the entitlement will expire
        /// </summary>
        public required DateTime ExpirationDate { get; set; }
        /// <summary>
        /// The product the entitlement is associated with
        /// </summary>
        public required string ProductCode { get; set; }
        /// <summary>
        /// Populated when the dimension value is a string. Otherwise default(string).
        /// </summary>
        public string? StringValue { get; set; }
        /// <summary>
        /// Populated when the dimension value is a double. Otherwise default(double).
        /// </summary>
        public double? DoubleValue { get; set; }
        /// <summary>
        /// Populated when the dimension value is a boolean. Otherwise default(bool).
        /// </summary>
        public bool? BooleanValue { get; set; }
        /// <summary>
        /// Populated when the dimension value is an integer. Otherwise default(int).
        /// </summary>
        public int? IntegerValue { get; set; }
    }
}
