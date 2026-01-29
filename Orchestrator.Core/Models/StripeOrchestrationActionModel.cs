
using System.Collections.Generic;

namespace Orchestrator.Core.Models
{
    public class StripeOrchestrationActionModel : OrchestrationActionBaseModel
    {
        public override Marketplace OriginatingMarketplace => Marketplace.Stripe;

        /// <summary>
        /// List of items on the subscription
        /// </summary>
        public List<StripeItemModel> Items { get; set; } = [];

        /// <summary>
        /// Custom fields entered on the Stripe checkout form.
        /// Will be empty if no custom fields are defined.
        /// </summary>
        public List<StripeCustomFieldModel> CustomStripeFields { get; set; } = [];
    }

    /// <summary>
    /// Model representing the items on a Stripe subscription.
    /// An item represents a certain number of products purchased at a certain price.
    /// </summary>
    public class StripeItemModel
    {
        /// <summary>
        /// Id of the line item for this subscription
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// The Id of the Price associated with this item
        /// </summary>
        public required string PriceId { get; set; }

        /// <summary>
        /// The Product Id
        /// </summary>
        public required string ProductId { get; set; }

        /// <summary>
        /// The friendly name of the Product
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of the item on the subscription
        /// </summary>
        public long Quantity { get; set; } = 0;
    }

    /// <summary>
    /// Custom field completed by the user during Stripe checkout
    /// </summary>
    public class StripeCustomFieldModel
    {
        /// <summary>
        /// An ID for the custom field
        /// </summary>
        public required string Key { get; set; }
        /// <summary>
        /// The value of the custom field
        /// </summary>
        public string? Value { get; set; }
        /// <summary>
        /// A user-friendly label for the custom field
        /// </summary>
        public string Label { get; set; } = string.Empty;
    }
}
