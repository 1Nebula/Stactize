using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Core.Contracts;
using System.Text.Json.Serialization;
using System.Text.Json;
using Orchestrator.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<IServiceBusService, ServiceBusService>();

        services.Configure<JsonSerializerOptions>(options =>
        {
            // This allows the orchestrator function to handle the string names of enum values in the SubscriptionEvent enum
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNameCaseInsensitive = true;

            // This allows the orchestrator function to ignore comments in the json messages it receives
            options.ReadCommentHandling = JsonCommentHandling.Skip;
        });
    })
    .Build();

host.Run();
