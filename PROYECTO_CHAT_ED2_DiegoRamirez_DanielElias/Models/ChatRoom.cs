using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models
{
    public class ChatRoom
    {
        [BsonId]
        public string id { get; set; }

        public string GroupName { get; set; }

        public List<string> chatMembers { get; set; }

        public List<Messages> messagesList { get; set; }

        public List<string> keys { get; set; }
    }
}
