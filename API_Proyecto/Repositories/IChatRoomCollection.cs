using API_Proyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Proyecto.Repositories
{
    interface IChatRoomCollection
    {
        Task NewChatroom(ChatRoom chat);

        Task EditChat(ChatRoom chat);

        Task<List<ChatRoom>> GetAllChatRooms();

        Task<ChatRoom> GetChatRoomById(string id);
    }
}
