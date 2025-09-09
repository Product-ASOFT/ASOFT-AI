using ASOFT.Core.Common.InjectionChecker;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ASOFT.Core.Common.Security.Identity.Jwt
{
    /// <summary>
    /// Default class để tạo token
    /// </summary>
    public class JwtHelper : IJwtHelper
    {
        public const string AccessToken = "access_token";

        private readonly JwtSettings _jwtSettings;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtHelper(JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
            _jwtSettings = Checker.NotNull(jwtSettings, nameof(jwtSettings));
            _tokenValidationParameters = Checker.NotNull(tokenValidationParameters, nameof(tokenValidationParameters));
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public virtual string CreateSecurityToken(DateTime expiresTime, IEnumerable<Claim> claims = null)
        {
            Checker.NotNull(expiresTime, nameof(expiresTime));

            var signingCredentials = new SigningCredentials(_tokenValidationParameters.IssuerSigningKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                expires: expiresTime,
                signingCredentials: signingCredentials,
                claims: claims);

            return _tokenHandler.WriteToken(token);
        }

        public virtual void ValidationToken(string token, out SecurityToken securityToken)
        {
            Checker.NotEmpty(token, nameof(token));

            _tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken scToken);
            securityToken = scToken;
        }

        public virtual JwtSecurityToken DecodeToken(string encode)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(encode) as JwtSecurityToken;
                return token;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Cannot decode the token: {encode}.", ex);
            }
        }
    }
}