using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias
{
    public class Class : Hub // "Hub" is abstract class  under Microsoft.AspNetCore.SignalR namespace
    {
        public async Task SendMessage(string message, string userId)
        {
            await Clients.Clients(userId).SendAsync("ReceiveMessage", message, Context.ConnectionId);
            await Clients.Clients(Context.ConnectionId).SendAsync("OwnMessage", message.Trim());
        }
        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            Clients.All.SendAsync("OnlineUserList", connectionId);
            return base.OnConnectedAsync();
        }
        public async Task OnlineUsers()
        {
            var connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("OnlineUserList", connectionId);
        }
    }
}
