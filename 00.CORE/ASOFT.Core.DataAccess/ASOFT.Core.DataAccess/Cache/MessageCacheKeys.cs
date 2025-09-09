using ASOFT.Core.Common.InjectionChecker;
using System;

namespace ASOFT.Core.DataAccess.Cache
{
    public static class MessageCacheKeys
    {
        private static string MessagePrefixKey => "ASOFT:Common:Message";

        public static string CreateMessageCacheKey(string messageName, string culture)
        {
            Checker.NotEmpty(messageName, nameof(messageName));
            Checker.NotEmpty(culture, nameof(culture));
            return $"{MessagePrefixKey}.{culture}.{messageName}";
        }

        public static string CreateResourceTypeCacheKey(Type resourceType)
        {
            Checker.NotNull(resourceType, nameof(resourceType));
            return $"{MessagePrefixKey}.{resourceType.FullName}";
        }

        public static string CreateBaseNameLocationCacheKey(string baseName, string location)
        {
            Checker.NotEmpty(baseName, nameof(baseName));
            Checker.NotEmpty(location, nameof(location));
            return $"{MessagePrefixKey}.{baseName}{location}";
        }
    }
}