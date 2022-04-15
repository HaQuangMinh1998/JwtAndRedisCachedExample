using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services.Interfaces
{
    public interface IChatHubService 
    {
        public Task OnConnectedAsync();
        public Task OnDisconnectedAsync(Exception exception);
        public Task SendToUser(string user, string message);
        public Task SendToAll(string user, string message);
    }
}
