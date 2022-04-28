using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Models;
using WebClient.Services;
using WebClient.Services.Interfaces;

namespace WebClient.Controllers
{
    public class ChatHubController : Controller
    {
        private readonly ILogger<ChatHubController> _logger;
        private readonly IHubContext<ChatHubService> _hubContext;

        public ChatHubController(ILogger<ChatHubController> logger,
            IHubContext<ChatHubService> hubContext)
        {
            this._logger = logger;
            this._hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task<IActionResult> send1()
        {
            await this._hubContext.Clients.All.SendAsync("ReceiveMessage2", "user", "message" );
            return Ok();
        }
    }
}

