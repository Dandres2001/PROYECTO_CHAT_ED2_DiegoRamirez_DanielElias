using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string roomId, string user, string message)
        {
            await Clients.Group(roomId).SendAsync("RecieveMessage", user, message);


        }

        public async Task AddToGroup(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);


        }
    }
}
