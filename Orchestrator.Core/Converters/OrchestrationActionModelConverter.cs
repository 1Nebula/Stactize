using Orchestrator.Core.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orchestrator.Core.Converters
{
    /// <summary>
    /// Custom Json Converter that converts an OrchestrationActionBaseModel to the relevant
    /// marketplace-specific Orchestration Action Model based on the OriginatingMarketplace property
    /// </summary>
    public class OrchestrationActionModelConverter : JsonConverter<OrchestrationActionBaseModel>
    {
        public override OrchestrationActionBaseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Load the raw JSON into a JsonDocument
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("OriginatingMarketplace", out var marketplaceProperty))
            {
                throw new JsonException("Missing OriginatingMarketplace property.");
            }

            var originatingMarketplace = marketplaceProperty.GetString();
            OrchestrationActionBaseModel result = originatingMarketplace switch
            {
                "Azure" => JsonSerializer.Deserialize<AzureOrchestrationActionModel>(jsonDoc, options),
                "Aws" => JsonSerializer.Deserialize<AwsOrchestrationActionModel>(jsonDoc, options),
                "Stripe" => JsonSerializer.Deserialize<StripeOrchestrationActionModel>(jsonDoc, options),
                _ => throw new JsonException($"Unknown OriginatingMarketplace: {originatingMarketplace}")
            };

            return result!;
        }

        public override void Write(Utf8JsonWriter writer, OrchestrationActionBaseModel value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
