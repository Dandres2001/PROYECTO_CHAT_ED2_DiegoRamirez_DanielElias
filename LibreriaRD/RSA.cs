using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace LibreriaRD
{
   public class RSA
    {
        public List<string> GenerarLlaves()
        {
            Random rand = new Random();
            int p = 0;
            int q = 0;
            bool stop = false;
            while (stop == false)
            {
                p = rand.Next(280, 700);
                q = rand.Next(280, 700);
                if (EsPrimo(p) == true && EsPrimo(q) == true)
                {
                    stop = true;
                }


            }
            int n = p * q;
            int Fi_n = (p - 1) * (q - 1);
            int i;
            List<string> llaves = new List<string>();
            for (i = 2; i < Fi_n; i++)
            {
                if (coprime(Fi_n, i) == true && coprime(n, i) == true)
                {
                    break;
                }

            }
            int e = i;
            int d = calcularD(e, Fi_n);
            if (e == d)
            {
                e = e + n;

            }
            llaves.Add(n.ToString());
            llaves.Add(e.ToString());
            llaves.Add(d.ToString());
            return llaves;
            //public_key = Convert.ToString(e) + "," + Convert.ToString(n);
            //private_key = Convert.ToString(d) + "," + Convert.ToString(n);

        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public string RSA_CYPHER(string mensaje, int n, int k)
        {
            var numeros = new List<int>();
            byte[] numerosmensaje = GetBytes(mensaje);
            foreach (int b in numerosmensaje)
            {

                int cifrado = (int)BigInteger.ModPow(b, k, n);
                numeros.Add(cifrado);

            }
            byte[] mensajefinal = numeros.SelectMany(BitConverter.GetBytes).ToArray();
            string charsfinales = GetString(mensajefinal);
            return charsfinales;

        }
        public string RSA_DECYPHER(string mensaje, int n, int k)
        {

            byte[] numeros = GetBytes(mensaje);

            var numerosmensaje = new int[numeros.Length / 4];
            Buffer.BlockCopy(numeros, 0, numerosmensaje, 0, numeros.Length);

            List<byte> mensajedescifrado = new List<byte>();

            foreach (int b in numerosmensaje)
            {

                int cifrado = (int)BigInteger.ModPow(b, k, n);

                mensajedescifrado.Add((byte)cifrado);
            }
            string mensajefinal = GetString(mensajedescifrado.ToArray());
            return mensajefinal;
        }





        static int calcularD(int a, int m)
        {

            for (int x = 1; x < m; x++)
                if (((a % m) * (x % m)) % m == 1)
                    return x;
            return 1;
        }
        public bool EsPrimo(int numero)
        {
            for (int i = 2; i < numero; i++)
            {
                if ((numero % i) == 0)
                {
                    //no primo 
                    return false;
                }
            }

            //primo
            return true;
        }

        static bool coprime(long u, long v)
        {
            if (((u | v) & 1) == 0) return false;

            while ((u & 1) == 0) u >>= 1;
            if (u == 1) return true;

            do
            {
                while ((v & 1) == 0) v >>= 1;
                if (v == 1) return true;

                if (u > v) { long t = v; v = u; u = t; }
                v -= u;
            } while (v != 0);

            return false;
        }

       

    }
}
