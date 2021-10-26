using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models
{
    public class Users
    {
        [BsonId]
        public string guid { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string eMail { get; set; }

        public List<Users> friendsList { get; set; }

        public List<Users> requestsList { get; set; }

        

    }
}
