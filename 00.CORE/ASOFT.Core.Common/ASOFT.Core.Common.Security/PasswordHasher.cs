using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace ASOFT.Core.Common.Security
{
    /// <summary>
    /// Default Password hasher
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA256; // default for Rfc2898DeriveBytes
        private const int Pbkdf2IteratorCount = 8888; // default for Rfc2898DeriveBytes
        private const int Pbkdf2SubKeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits
        private const int ExtraLength = 8;

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <history>
        ///     [Luan Le] Created [22/07/2019]
        /// </history>
        public virtual string Encrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"{nameof(password)} cannot be null or empty.");

            return HashPassword(password);
        }

        /// <summary>
        /// Chứng thực password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        /// <history>
        ///     [Luan Le] Created [22/07/2019]
        /// </history>
        public virtual bool VerifyHash(string hashedPassword, string password)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                throw new ArgumentException($"{nameof(hashedPassword)} cannot be null or empty.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"{nameof(password)} cannot be null or empty.");

            return VerifyHashedPassword(hashedPassword, password);
        }

        /// <summary>
        ///     Mã hóa chuỗi
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            ASOFTSecurityStaticVariables.RandomNumberGenerator.GetBytes(salt);

            byte[] subkey =
                KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IteratorCount, Pbkdf2SubKeyLength);

            var outputBytes = new byte[ExtraLength + SaltSize + Pbkdf2SubKeyLength];
            outputBytes[0] = 0x00; // format marker

            Buffer.BlockCopy(salt, 0, outputBytes, ExtraLength, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, ExtraLength + SaltSize, Pbkdf2SubKeyLength);

            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        ///     So sánh chuỗi mã hóa vs chuỗi người dùng
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <history>
        ///     [Luan Le] Created [30/07/2019]
        /// </history>
        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            var hashedBytes = Convert.FromBase64String(hashedPassword);

            // We know ahead of time the exact length of a valid hashed password payload.
            if (hashedBytes.Length != ExtraLength + SaltSize + Pbkdf2SubKeyLength)
            {
                return false; // bad size
            }

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedBytes, ExtraLength, salt, 0, salt.Length);

            var expectedSubkey = new byte[Pbkdf2SubKeyLength];
            Buffer.BlockCopy(hashedBytes, ExtraLength + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            var actualSubkey =
                KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IteratorCount, Pbkdf2SubKeyLength);

            return ByteArraysEqual(actualSubkey, expectedSubkey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }

            return areSame;
        }
    }
}