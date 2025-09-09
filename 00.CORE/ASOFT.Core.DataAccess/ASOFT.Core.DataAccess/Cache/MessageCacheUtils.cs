using System;

namespace ASOFT.Core.DataAccess.Cache
{
    public static class MessageCacheUtils
    {
        public static DateTimeOffset GetMessageCacheAbsoluteExpiration(object value) =>
            value == null ? DateTimeOffset.Now.AddMinutes(5) : DateTimeOffset.Now.AddDays(7);
    }
}