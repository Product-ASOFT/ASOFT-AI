using Microsoft.AspNetCore.Mvc;
using System;

namespace ASOFT.Core.API.Versions
{
    /// <summary>
    /// Api Version 1 attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ApiV1Attribute : ApiVersionAttribute
    {
        /// <summary>
        /// Version 1 attribute
        /// </summary>
        public ApiV1Attribute() : base(SupportApiVersions.V_1_0)
        {
        }
    }

    /// <summary>
    /// Api Version 2 attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ApiV2Attribute : ApiVersionAttribute
    {
        /// <summary>
        /// Version 2 attribute
        /// </summary>
        public ApiV2Attribute() : base(SupportApiVersions.V_2_0)
        {
        }
    }
}