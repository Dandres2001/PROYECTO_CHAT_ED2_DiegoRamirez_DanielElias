﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Proyecto.Models
{
    public class Messages
    {

        public string id { get; set; }
        public string Text { get; set; }

        public string SenderUsername { get; set; }

        public List<string> Readers { get; set; }

        public string date { get; set; }

        public byte[] File { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public bool isFile { get; set; }
    }
}
