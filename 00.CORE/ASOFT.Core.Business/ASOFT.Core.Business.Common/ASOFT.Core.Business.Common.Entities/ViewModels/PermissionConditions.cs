using System;

namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    public static class PermissionConditions
    {
        public const string VT = "VT";
        public const string AC = "AC";
        public const string IV = "IV";
        public const string OB = "OB";
        public const string WA = "WA";
        public const string LP = "LP";
        public const string DE = "DE";

        private const string PREFIX = "ASOFT";

        public static string FormatAsModuleID(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                return string.Empty;
            }

            if (PREFIX.IndexOf(moduleId, StringComparison.Ordinal) == 0)
            {
                return moduleId;
            }

            return $"{PREFIX}{moduleId}";
        }
    }
}