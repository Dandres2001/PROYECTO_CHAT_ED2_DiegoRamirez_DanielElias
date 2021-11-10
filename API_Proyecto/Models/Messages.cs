using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Proyecto.Models
{
    public class Messages
    {
        public string Text { get; set; }

        public string SenderUsername { get; set; }

        public List<string> Readers { get; set; }
    }
}
