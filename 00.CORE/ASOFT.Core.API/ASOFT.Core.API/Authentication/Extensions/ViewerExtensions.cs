using ASOFT.Core.Common.Contract;
using JetBrains.Annotations;

namespace ASOFT.Core.API.Authentication.Extensions
{
    /// <summary>
    /// Extensions cho viewer
    /// </summary>
    public static class ViewerExtensions
    {
        /// <summary>
        /// Lấy ngôn ngữ
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns></returns>
        public static string LanguageID([NotNull] this IViewer viewer) =>
            Checker.NotNull(viewer, nameof(viewer)).Culture.Name;
    }
}