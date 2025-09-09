using JetBrains.Annotations;
using System.Globalization;

namespace ASOFT.Core.Common.Security.Identity
{
    /// <summary>
    /// Class cho lấy thông tin chứng thực của người dùng, có thể custom.
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// The id of current user.
        /// </summary>
        [CanBeNull]
        string ID { get; }

        /// <summary>
        /// The name of current user.
        /// </summary>
        [CanBeNull]
        string Name { get; }

        /// <summary>
        /// Culture of current user.
        /// </summary>
        [NotNull]
        CultureInfo Culture { get; }
    }
}