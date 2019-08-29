using LearningSignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LearningSignalR
{
    public class LearningHub : Hub<ILearningHubClient>
    {
        [Authorize]
        public Task BroadcastMessage(string message)
        {
            return Clients.All.ReceiveMessage(GetMessageWithName(message));
        }

        public async Task SendToCaller(string message)
        {
            await Clients.Caller.ReceiveMessage(GetMessageWithName(message));
        }

        public async Task SendToOthers(string message)
        {
            await Clients.Others.ReceiveMessage(GetMessageWithName(message));
        }

        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(GetMessageWithName(message));
        }

        public async Task SendToUser(string userName, string message)
        {
            await Clients.Group(userName).ReceiveMessage(GetMessageWithName(message));
        }

        [Authorize(Roles = "Admin")]
        public async Task AddUserToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"Current user added to {groupName} group");
            await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} added to {groupName} group");
        }

        [Authorize(Roles = "Admin")]
        public async Task RemoveUserFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"Current user removed from {groupName} group");
            await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} removed from {groupName} group");
        }

        [Authorize]
        public Task BroadcastObject(MessagePayload payload)
        {
            return Clients.All.ReceiveMessage(GetObjectWithName(payload));
        }

        public async Task SendObjectToCaller(MessagePayload payload)
        {
            await Clients.Caller.ReceiveMessage(GetObjectWithName(payload));
        }

        public async Task SendObjectToOthers(MessagePayload payload)
        {
            await Clients.Others.ReceiveMessage(GetObjectWithName(payload));
        }

        public async Task SendObjectToGroup(string groupName, MessagePayload payload)
        {
            await Clients.Group(groupName).ReceiveMessage(GetObjectWithName(payload));
        }

        public async Task SendObjectToUser(string userName, MessagePayload payload)
        {
            await Clients.Group(userName).ReceiveMessage(GetObjectWithName(payload));
        }

        public override async Task OnConnectedAsync()
        {
            if (Context?.User?.Identity?.Name != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);

            await Groups.AddToGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context?.User?.Identity?.Name != null)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnDisconnectedAsync(exception);
        }

        public ChannelReader<int> Counter(
        int count,
        int delay,
        CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<int> writer,
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            try
            {
                for (var i = 0; i < count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await writer.WriteAsync(i);
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
            }

            writer.TryComplete();
        }

        private string GetMessageWithName(string message)
        {
            if (Context?.User?.Identity?.Name != null)
                return $"{Context.User.Identity.Name} said: {message}";

            return message;
        }

        private string GetObjectWithName(MessagePayload payload)
        {
            var message = JsonConvert.SerializeObject(payload);

            if (Context?.User?.Identity?.Name != null)
                return $"{Context.User.Identity.Name} said: {message}";

            return message;
        }
    }
}
