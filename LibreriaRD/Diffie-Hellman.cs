using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace LibreriaRD
{
    public class Diffie_Hellman
    {

        public List<int> calcularPG()
        {
            List<int> PG = null;
            Random rand = new Random();
            int modulus = 0;
            bool correcto = false;
            while (correcto != true)
            {
                modulus = rand.Next(256, 1000);
                if (EsPrimo(modulus) == true)
                {
                    correcto = true;
                }

            }
            int generator = rand.Next(2, 100);
            PG.Add(modulus);
            PG.Add(generator);
            int a = rand.Next(3, modulus - 1);
            int b = rand.Next(3, modulus - 1);
            if (a == b)
            {
                a = a - 1;
            }
            PG.Add(a);
            PG.Add(a);

            return PG;
        }

        public List<string> getpublickey()
        {
            List<string> publickeys = new List<string>();
     
            Random rand = new Random();
            int modulus = 0;
            bool correcto = false;
            while (correcto != true)
            {
                modulus = rand.Next(256, 1000);
                if (EsPrimo(modulus) == true)
                {
                    correcto = true;
                }

            }
            int generator = rand.Next(2, 100);
            publickeys.Add(modulus.ToString()); //0
            publickeys.Add(generator.ToString()); //1
            int a = rand.Next(3, modulus - 1);
            int b = rand.Next(3, modulus - 1);
            if (a == b)
            {
                a = a - 1;
            } 
            publickeys.Add(a.ToString());  //2
            publickeys.Add(b.ToString());  //3
            int A = (int)BigInteger.ModPow(generator,a , modulus);
            int B = (int)BigInteger.ModPow(generator, b, modulus);

            publickeys.Add(A.ToString());  //4
            publickeys.Add(B.ToString()); //5

            return publickeys; 
        }


        public string getprivatekey(List<string> publickeys)
        {
            int k1 = (int)BigInteger.ModPow(Convert.ToInt32(publickeys[5]), Convert.ToInt32(publickeys[2]), Convert.ToInt32(publickeys[0]));

            int k2 = (int)BigInteger.ModPow(Convert.ToInt32(publickeys[4]), Convert.ToInt32(publickeys[3]), Convert.ToInt32(publickeys[0]));

            if  (k1 == k2)
            {
                return k1.ToString();

            }
            return null;
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

  
    }
}
