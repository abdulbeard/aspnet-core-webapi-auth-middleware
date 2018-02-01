using System;
using System.Security.Cryptography;

namespace MisturTee.Utils
{
    public class CryptoUtils
    {
        public static byte[] GetSymmetricKey()
        {
            var byteArray = new byte[128];
            new RNGCryptoServiceProvider().GetBytes(byteArray);
            return byteArray;
        }

        public static string GetSymmetricKey_Base64()
        {
            return Convert.ToBase64String(GetSymmetricKey());
        }
    }
}
