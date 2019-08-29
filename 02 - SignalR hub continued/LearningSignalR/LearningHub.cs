using LearningSignalR.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace LearningSignalR
{
    public class LearningHub  : Hub
    {
        public Task BroadcastMessage(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendToCaller(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public async Task SendToOthers(string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", message);
        }

        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task AddUserToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("ReceiveMessage", $"Current user added to {groupName} group");
            await Clients.Others.SendAsync("ReceiveMessage", $"User {Context.ConnectionId} added to {groupName} group");
        }

        public async Task RemoveUserFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("ReceiveMessage", $"Current user removed from {groupName} group");
            await Clients.Others.SendAsync("ReceiveMessage", $"User {Context.ConnectionId} removed from {groupName} group");
        }

        public Task BroadcastObject(MessagePayload payload)
        {
            return Clients.All.SendAsync("ReceiveObject", payload);
        }

        public async Task SendObjectToCaller(MessagePayload payload)
        {
            await Clients.Caller.SendAsync("ReceiveObject", payload);
        }

        public async Task SendObjectToOthers(MessagePayload payload)
        {
            await Clients.Others.SendAsync("ReceiveObject", payload);
        }

        public async Task SendObjectToGroup(string groupName, MessagePayload payload)
        {
            await Clients.Group(groupName).SendAsync("ReceiveObject", payload);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
