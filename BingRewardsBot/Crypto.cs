using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BingRewardsBot
{
    class Crypto
    {
        private string key;
        public Crypto(string k)
        {
            key = k;
        }
        public string encrypt(string input)
        {
            byte[] input_bytes = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider td = new TripleDESCryptoServiceProvider();
            td.Key = UTF8Encoding.UTF8.GetBytes(key);
            td.Mode = CipherMode.ECB;
            td.Padding = PaddingMode.PKCS7;
            ICryptoTransform ct = td.CreateEncryptor();
            byte[] result_bytes = ct.TransformFinalBlock(input_bytes, 0, input_bytes.Length);
            td.Clear();
            string result = Convert.ToBase64String(result_bytes, 0, result_bytes.Length);
            return result;
        }

        public string decrypt(string input)
        {
            byte[] input_bytes = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider td = new TripleDESCryptoServiceProvider();
            td.Key = UTF8Encoding.UTF8.GetBytes(key);
            td.Mode = CipherMode.ECB;
            td.Padding = PaddingMode.PKCS7;
            ICryptoTransform ct = td.CreateDecryptor();
            byte[] result_bytes = ct.TransformFinalBlock(input_bytes, 0, input_bytes.Length);
            td.Clear();
            string result = UTF8Encoding.UTF8.GetString(result_bytes, 0, result_bytes.Length);
            return result;
        }
    }
}
