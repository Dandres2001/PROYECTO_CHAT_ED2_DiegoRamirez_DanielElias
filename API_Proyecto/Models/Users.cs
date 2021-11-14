using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace API_Proyecto.Models
{
    public class Users
    {
        [BsonId]
        public ObjectId id { get; set; }
   
        public string Username { get; set; }

        public string Password { get; set; }

        public string eMail { get; set; }

        public string key { get; set; }
        public List<string> friendsList { get; set; }

        public List<string> requestsList { get; set; }

        public Dictionary<string,string> ChatRoomsIds { get; set; }
    }
}
