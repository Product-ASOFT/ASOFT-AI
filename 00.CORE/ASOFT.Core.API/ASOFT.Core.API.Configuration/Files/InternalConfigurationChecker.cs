using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using System;
using System.IO;

namespace ASOFT.Core.API.Configuration.Files
{
    internal static class InternalConfigurationChecker
    {
        internal static void FileMustBeExisted(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cannot find file with path `{filePath}`", filePath);
            }
        }

        internal static void MakeSureExtensionFileValid(string path, [NotNull] string fileExtension)
        {
            Checker.NotEmpty(fileExtension, nameof(fileExtension));

            var pathExtension = Path.GetExtension(path);
            if (!fileExtension.Equals(pathExtension, StringComparison.Ordinal))
            {
                throw new ArgumentException($"File with path `{path}` does not has extension `{fileExtension}`");
            }
        }
    }
}