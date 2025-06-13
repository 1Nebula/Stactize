namespace Orchestrator.Core.Models
{
    /// <summary>
    /// Orchestration Action Model for Azure events
    /// </summary>
    public class AzureOrchestrationActionModel : OrchestrationActionBaseModel
    {
        /// <summary>
        /// Override for Azure marketplace orchestration actions
        /// </summary>
        public override Marketplace OriginatingMarketplace => Marketplace.Azure;

        /// <summary>
        /// The Id of the selected plan (as configured in the Application configuration)
        /// </summary>
        public string PlanId { get; set; } = string.Empty;
    }
}