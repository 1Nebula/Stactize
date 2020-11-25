using System.Threading.Tasks;

namespace Orchestrator.Core.Contracts
{
    public interface IServiceBusService
    {
        Task SendResultToStactize(OrchestrationResultModel result);
    }
}
