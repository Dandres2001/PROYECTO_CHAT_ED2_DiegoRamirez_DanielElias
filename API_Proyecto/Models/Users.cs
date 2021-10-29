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

        public List<Users> friendsList { get; set; }

        public List<Users> requestsList { get; set; }
    }
}
