using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using Orchestrator.Core.Converters;
using Microsoft.Azure.Functions.Worker.Builder;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    // This allows the orchestrator function to handle the string names of enum values in the SubscriptionEvent enum
    options.Converters.Add(new JsonStringEnumConverter());
    // Allow conversion between multiple marketplace models
    options.Converters.Add(new OrchestrationActionModelConverter());
    options.PropertyNameCaseInsensitive = true;

    // This allows the orchestrator function to ignore comments in the json messages it receives
    options.ReadCommentHandling = JsonCommentHandling.Skip;
});

builder.Services.AddServiceBusService(builder.Configuration);


await builder.Build().RunAsync();