using DurableFunctionOrchestratorExample;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrator.Core.Contracts;
using Orchestrator.Services;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DurableFunctionOrchestratorExample
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Use the IFunctionsHostBuilder to configure your application and add services for dependency injection
            var serviceCollection = builder.Services;

            serviceCollection.AddTransient<IServiceBusService, ServiceBusService>();
        }
    }
}
