using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models
{
    public class Messages
    {
        public string id { get; set; }
        public string Text { get; set; }

        public string SenderUsername { get; set; }

        public List<string> Readers { get; set; }

        public string date { get; set; }
    }
}
