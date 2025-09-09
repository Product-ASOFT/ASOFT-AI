using System.Collections.Generic;
using System.Text;

namespace ASOFT.Core.Common.Security
{
    /// <summary>
    /// Random class helper.
    /// </summary>
    public static class ASOFTRandomHelper
    {
        /// <summary>
        /// Tạo một giá trị duy nhất.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateUniqueValue(int length = 32)
        {
            var bytes = new byte[length];
            ASOFTSecurityStaticVariables.RandomNumberGenerator.GetBytes(bytes);
            return ByteArrayToString(bytes);
        }

        private static string ByteArrayToString(IReadOnlyCollection<byte> bytes)
        {
            var hex = new StringBuilder(bytes.Count * 2);
            foreach (var b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}