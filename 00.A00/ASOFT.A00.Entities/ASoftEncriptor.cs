// #################################################################
// # Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.
// #
// # History：
// #    Date Time           Created        	Content
// #    04/12/2020          Tấn Lộc        	Tạo mới
// ##################################################################

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;



namespace ASOFT.A00.Entities
{
    public class ASoftEncriptor
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        // DO NOT CHANGE THIS CONST, IT WILL CHANGE DECRIPTED VALUE
        private const string initVector = "AsoftEncryptorIV";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        /// <summary>
        /// Encripted Plaintext
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Lộc] created on [04/12/2020]
        /// </history>
        public static string Encrypt(string plainText, string passPhrase)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            byte[] saltArray = Encoding.ASCII.GetBytes(passPhrase);
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltArray);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            Aes symmetricKey = Aes.Create();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Dispose();
            cryptoStream.Dispose();
            return Convert.ToBase64String(cipherTextBytes);

            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            //return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decripted Plaintext
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Lộc] created on [04/12/2020]
        /// </history>
        public static string Decrypt(string cipherText, string passPhrase)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            byte[] saltArray = Encoding.ASCII.GetBytes(passPhrase);
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltArray);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            Aes symmetricKey = Aes.Create();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Dispose();
            cryptoStream.Dispose();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            //var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            //return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
