using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orchestrator.Core;
using Orchestrator.Core.Constants;
using Orchestrator.Core.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _serviceBusSender;

        public ServiceBusService(IConfiguration configuration)
        {
            _serviceBusClient = new ServiceBusClient(configuration[ConfigurationConstants.egressConnectionString]);
            _serviceBusSender = _serviceBusClient.CreateSender(configuration[ConfigurationConstants.egressQueueName]);
        }

        public async Task SendResultToStactize(OrchestrationResultModel result)
        {
            await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result))));
        }
    }
}
