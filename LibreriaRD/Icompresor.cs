using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD
{
  public  interface Icompresor
    {
        public byte[] Compress(byte[] uncompressed);

        public byte[] Decompress(byte[] diccionario, byte[] mensaje);
    }
}
