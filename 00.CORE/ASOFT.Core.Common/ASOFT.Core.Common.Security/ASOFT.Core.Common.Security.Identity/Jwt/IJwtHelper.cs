using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ASOFT.Core.Common.Security.Identity.Jwt
{
    /// <summary>
    /// Helper class giúp tạo token
    /// </summary>
    public interface IJwtHelper
    {
        /// <summary>
        /// Tạo một token
        /// </summary>
        /// <param name="expiresTime"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        string CreateSecurityToken(DateTime expiresTime, IEnumerable<Claim> claims = null);

        /// <summary>
        /// Validation token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="securityToken"></param>
        void ValidationToken(string token, out SecurityToken securityToken);

        /// <summary>
        /// Decode token
        /// </summary>
        /// <param name="encode"></param>
        /// <returns></returns>
        JwtSecurityToken DecodeToken(string encode);
    }
}