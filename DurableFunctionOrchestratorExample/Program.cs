using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Core.Contracts;
using Orchestrator.Services;
using System.Text.Json.Serialization;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        // Use the IServiceCollection to configure your application and add services for dependency injection
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<IServiceBusService, ServiceBusService>();

        // This will allow the orchestrator function to handle the friendly names of enum values in the SubscriptionEvent enum
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
            options.PropertyNameCaseInsensitive = true;
        });
    })
    .Build();

host.Run();