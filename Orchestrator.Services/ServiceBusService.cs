using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orchestrator.Core;
using Orchestrator.Core.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly IQueueClient _queueClient;

        public ServiceBusService(IConfiguration configuration)
        {
            _queueClient = new QueueClient(configuration["egressServiceBusConnectionString"], "orchestratorreadqueue");
        }

        public async Task SendResultToStactize(OrchestrationResultModel result)
        {
            await _queueClient.SendAsync(new Message(UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result))));
        }
    }
}
