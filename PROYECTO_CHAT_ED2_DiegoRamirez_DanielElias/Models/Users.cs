using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models
{
    public class Users
    {
        [BsonId]
        public ObjectId  id { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string eMail { get; set; }

        public string key { get; set; }

        public List<string> friendsList { get; set; }

        public List<string> requestsList { get; set; }

        public Dictionary<string, string> ChatRoomsIds { get; set; }


    }
}
