using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using WebClient.Services.Interfaces;

namespace WebClient.Services
{
    public class ChatHubService : Hub,IChatHubService
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveSystemMessage", $"{Context.UserIdentifier} joined.");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("ReceiveSystemMessage", $"{Context.UserIdentifier} left.");
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendToUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveDirectMessage", $"{Context.UserIdentifier}: {message}");
        }
        public async Task SendToAll(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage2", user, message);
        }
    }
}
