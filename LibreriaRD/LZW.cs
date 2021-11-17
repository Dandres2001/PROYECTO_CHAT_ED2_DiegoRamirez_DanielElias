using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace LibreriaRD
{
   public class LZW
    {
        public static byte[] ConvertToByte(int binary)
        {

            var list = new List<Byte>();


            while (binary > 255)
            {
                list.Add(Convert.ToByte(255));
                binary = binary - 255;
            }

            list.Add(Convert.ToByte(binary));



            return list.ToArray();
        }


        public byte[] Compress(byte[] uncompressed)
        {
            Dictionary<string, ushort> dictionary = new Dictionary<string, ushort>();
            string current = "";
            string currentcharacter = "";
            var compressed = new List<ushort>();

            byte[] separador = { default };
            int vf = uncompressed.Length;

            foreach (char z in uncompressed)
            {
                if (!dictionary.ContainsKey(z.ToString()))
                {
                    dictionary.Add(z.ToString(), (ushort)(dictionary.Count + 1));

                    currentcharacter += z.ToString();
                }
            }

            byte[] DiccionarioOriginal = currentcharacter.Select(Convert.ToByte).ToArray(); //Guarda el diccionario original
            byte[] largoDiccionario = ConvertToByte(DiccionarioOriginal.Length); //guarda el largo de ese diccionario 

            foreach (char c in uncompressed)
            {
                string chain = current + c;
                if (dictionary.ContainsKey(chain))
                {
                    current = chain;
                }
                else
                {
                    compressed.Add(dictionary[current]);

                    if (dictionary.Count == ushort.MaxValue)  //reinicia el diccionario
                    {
                        dictionary.Clear();
                        foreach (char z in currentcharacter)
                        {

                            dictionary.Add(z.ToString(), (ushort)(dictionary.Count + 1));
                        }

                    }
                    dictionary.Add(chain, (ushort)(dictionary.Count + 1));
                    current = c.ToString();
                }
            }


            if (!string.IsNullOrEmpty(current))
            {
                compressed.Add(dictionary[current]);
            }

            byte[] Compressedbytes = compressed.SelectMany(BitConverter.GetBytes).ToArray();


            byte[] mensajefinal = Combine(largoDiccionario.ToArray(), separador, DiccionarioOriginal.ToArray(), Compressedbytes);
            return mensajefinal;
        }






        public byte[] Decompress(byte[] diccionario, byte[] mensaje)
        {

            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();
            //llena el diccionario
            foreach (char c in diccionario)
            {

                dictionary.Add((ushort)(dictionary.Count + 1), c.ToString());

            }

            var numerosmensaje = new ushort[mensaje.Length / 2];
            Buffer.BlockCopy(mensaje, 0, numerosmensaje, 0, mensaje.Length);

            var previous = dictionary[numerosmensaje[0]];
            var decompressed = new StringBuilder(previous);


            numerosmensaje = numerosmensaje.Skip(1).ToArray();



            foreach (ushort k in numerosmensaje)
            {
                string entry = string.Empty;

                if (dictionary.ContainsKey(k))
                {
                    entry = dictionary[k];
                }
                else
                {
                    entry = previous + previous[0];
                }
                decompressed.Append(entry);
                if (dictionary.Count == ushort.MaxValue)  //reinicia el diccionario
                {
                    dictionary.Clear();
                    foreach (char c in diccionario)
                    {

                        dictionary.Add((ushort)(dictionary.Count + 1), c.ToString());

                    }

                }
                dictionary.Add((ushort)(dictionary.Count + 1), previous + entry[0]);

                previous = entry;

            }




            return decompressed.ToString().Select(Convert.ToByte).ToArray();

        }

        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
