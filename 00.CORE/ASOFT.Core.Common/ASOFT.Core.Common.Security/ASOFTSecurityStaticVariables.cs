using System;
using System.Security.Cryptography;
using System.Threading;

namespace ASOFT.Core.Common.Security
{
    /// <summary>
    /// Crypto variables
    /// </summary>
    public static class ASOFTSecurityStaticVariables
    {
        private static readonly Lazy<RandomNumberGenerator> s_RandomNumberGenerator =
            new Lazy<RandomNumberGenerator>(RandomNumberGenerator.Create, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Crypto random
        /// </summary>
        public static RandomNumberGenerator RandomNumberGenerator => s_RandomNumberGenerator.Value;
    }
}