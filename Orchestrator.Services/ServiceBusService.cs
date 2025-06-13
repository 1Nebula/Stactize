using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orchestrator.Core;
using Orchestrator.Core.Constants;
using Orchestrator.Core.Contracts;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Services
{

    public static class ServiceBusExtensions
    {
        public static IServiceCollection AddServiceBusService(this IServiceCollection services, IConfiguration configuration) 
        {
            var connectionString = configuration[ConfigurationConstants.egressConnectionString];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Unable to start orchestrator due to missing or invalid configuration at: {ConfigurationConstants.egressConnectionString}");

            services.AddAzureClients(c => 
            {
                c.AddServiceBusClient(connectionString);
            });
            services.AddSingleton<IServiceBusService, ServiceBusService>();
            return services;
        }
    }

    public class ServiceBusService : IServiceBusService
    {
        private readonly ServiceBusSender _serviceBusSender;

        public ServiceBusService(IConfiguration configuration, ServiceBusClient serviceBusClient)
        {
            ArgumentNullException.ThrowIfNull(serviceBusClient);
            _serviceBusSender = serviceBusClient.CreateSender(configuration[ConfigurationConstants.egressQueueName]);
        }

        public async Task SendResultToStactize(OrchestrationResultModel result)
        {
            await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result))));
        }
    }
}
