using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace API_Proyecto.Models
{
    public class ChatRoom
    {
        [BsonId]
        public string id { get; set; }

        public string GroupName { get; set; }

        public List<string> chatMembers { get; set; }

        public List<Messages> messagesList { get; set;  }

    }
}
