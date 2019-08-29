using LearningSignalR.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LearningSignalR
{
    public class LearningHub  : Hub
    {
        public Task BroadcastMessage(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", message);
        }

        public Task BroadcastObject(MessagePayload payload)
        {
            return Clients.All.SendAsync("ReceiveObject", payload);
        }
    }
}
