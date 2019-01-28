﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace RHOnline.Library
{
    public static class Criptografia
    {
        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.

            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
           
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            string st = sBuilder.ToString();
            
            //Invertemos a String
            char[] arrayChar = st.ToCharArray();
            Array.Reverse(arrayChar);
            return new string(arrayChar); 

        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(string input, string hash)
        {
            MD5 md5Hash = MD5.Create();

            // Hash the input.
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
