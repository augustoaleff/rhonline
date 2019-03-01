using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Library
{
    public class Shared
    {
        public static string PegarPrimeiroNome(string nome)
        {
            string primeiroNome = "";

            for (int i = 0; i < nome.Length; i++)
            {
                string letras = nome.Substring(i, 1);

                if (letras != " ")
                {
                    primeiroNome += letras;
                }
                else
                {
                    break;
                }

            }

            return primeiroNome;

        }

        public static string RetirarCaracteres(string input)
        {
            string output;

            input = input.Replace(".","");
            input = input.Replace(",", "");
            input = input.Replace("-", "");
            input = input.Replace("/", "");
            input = input.Replace("\\", "");
            input = input.Replace("|", "");
            input = input.Replace("@", "");
            input = input.Replace(" ", "");
            input = input.Replace("*", "");
            input = input.Replace("(", "");
            input = input.Replace(")", "");
            input = input.Replace("_", "");
            input = input.Replace("[", "");
            input = input.Replace("]", "");
            input = input.Replace(";", "");
            input = input.Replace(":", "");
            input = input.Replace("#", "");
            input = input.Replace("=", "");
            input = input.Replace("'", "");
            input = input.Replace("\"", "");
            input = input.Replace("~", "");
            input = input.Replace("^", "");
            input = input.Replace("%", "");
            input = input.Replace("$", "");
            input = input.Replace("!", "");
            input = input.Replace("¨", "");
            input = input.Replace("&", "");
            input = input.Replace("+", "");

            output = input.Trim();
            
            return output;
        }


        public enum Meses
        {
            Janeiro = 1,
            Fevereiro = 2,
            Março = 3,
            Abril = 4,
            Maio = 5,
            Junho = 6,
            Julho = 7,
            Agosto = 8,
            Setembro = 9,
            Outubro = 10,
            Novembro = 11,
            Dezembro = 12 
        }

    }
}
