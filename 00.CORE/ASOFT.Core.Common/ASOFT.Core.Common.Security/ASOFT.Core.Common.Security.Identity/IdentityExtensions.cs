using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;

namespace ASOFT.Core.Common.Security.Identity.Extensions
{
    /// <summary>
    /// Extensions cho viewer
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Lấy ngôn ngữ
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns></returns>
        public static string LanguageID([NotNull] this IIdentity viewer) =>
            Checker.NotNull(viewer, nameof(viewer)).Culture.Name;
    }
}