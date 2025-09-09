using ASOFT.Core.API.Httpss.Errors;
using System;

namespace ASOFT.Core.Business.Devices.Business.ErrorCodes
{
    public static class ClientVersionErrorCodes
    {
        public static readonly Lazy<ErrorCode> CheckVersionByUserAgentError =
            new Lazy<ErrorCode>(() => new ErrorCode(-1, nameof(CheckVersionByUserAgentError)));
    }
}