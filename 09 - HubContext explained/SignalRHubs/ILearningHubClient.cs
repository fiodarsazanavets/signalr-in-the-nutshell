using LearningSignalR.Models;
using System.Threading.Tasks;

namespace SignalRHubs
{
    public interface ILearningHubClient
    {
        Task ReceiveMessage(string message);
        Task ReceiveObject(MessagePayload payload);
    }
}
