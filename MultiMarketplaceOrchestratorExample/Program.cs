using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Core.Converters;
using Orchestrator.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

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
