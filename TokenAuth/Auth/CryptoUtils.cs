using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace TokenAuth.Auth
{
    public class CryptoUtils
    {
        private static readonly byte[] Key = Convert.FromBase64String("jApKAyYRMOQW1lilWun6RKEmn7914LmXlz0Oa2q5wCg=");
        private static readonly byte[] Iv = Convert.FromBase64String("H1Ykk45i/L66XaXv6Hh6Qg==");

        //public static byte[] GetSymmetricKey()
        //{
        //    var byteArray = new byte[128];
        //    new RNGCryptoServiceProvider().GetBytes(byteArray);
        //    return byteArray;
        //}

        //public static string GetSymmetricKey_Base64()
        //{
        //    return Convert.ToBase64String(GetSymmetricKey());
        //}

        public static string SymmetricallyEncryptString(string plainText)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            CheckKeyAndIv();
            var encrypted = new List<byte>();
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg != null)
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = Iv;
                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray().ToList();
                        }
                    }
                }
            }
            return Convert.ToBase64String(encrypted.ToArray());
        }

        public static string SymmetricallyDecryptString(string cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }
            CheckKeyAndIv();

            string plaintext = string.Empty;
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg != null)
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = Iv;
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

            } return plaintext;
        }

        private static void CheckKeyAndIv()
        {
            if (Key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(Key));
            }
            if (Iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(Iv));
            }
        }
    }
}
