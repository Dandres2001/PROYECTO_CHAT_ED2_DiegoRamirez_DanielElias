using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD
{
   public interface Icifrados
    {
        public string Cypher(string llave, string mensaje);

        public string Decypher(string llave, string mensaje);

        public string RSA_CYPHER(string mensaje, int n, int k);

        public string RSA_DECYPHER(string mensaje, int n, int k);


    }
}
