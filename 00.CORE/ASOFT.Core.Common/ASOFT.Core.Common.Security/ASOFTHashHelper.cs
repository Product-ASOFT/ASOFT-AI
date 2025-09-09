using System;
using System.Security.Cryptography;
using System.Text;

namespace ASOFT.Core.Common.Security
{
    /// <summary>
    /// Helper class giúp cho việc hash
    /// </summary>
    public static class ASOFTHashHelper
    {
        /// <summary>
        /// Hash với thuật toán SHA1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HashSHA1(byte[] value)
        {
            if (value == null)
            {
                return null;
            }

            using (var sha = SHA1.Create())
            {
                return sha.ComputeHash(value);
            }
        }

        /// <summary>
        /// Hash với thuật toán SHA1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HashSHA1(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = HashSHA1(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Hash với thuật toán SHA256.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HashSHA256(byte[] value)
        {
            if (value == null)
            {
                return null;
            }

            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(value);
            }
        }

        /// <summary>
        /// Hash với thuật toán SHA256.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HashSHA256(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = HashSHA256(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Hash với thuật toán SHA512.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HashSHA512(byte[] value)
        {
            if (value == null)
            {
                return null;
            }

            using (var sha = SHA512.Create())
            {
                return sha.ComputeHash(value);
            }
        }

        /// <summary>
        /// Hash với thuật toán SHA512
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HashSHA512(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = HashSHA512(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}