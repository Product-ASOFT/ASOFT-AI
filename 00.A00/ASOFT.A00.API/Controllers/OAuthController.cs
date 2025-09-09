using ASOFT.A00.Business.Interfaces;
using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.API.Core.Middleware;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.API.Versions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ASOFT.A00.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "A00")]
    public class OAuthController : A00BaseController
    {
        private readonly IJwtHandler _jwtHandler;
        private readonly IOAuthQueries _queries;
        private readonly IOAuthBusiness _business;
        public OAuthController(IJwtHandler jwtHandler, IOAuthQueries queries, IOAuthBusiness business)
        {
            _jwtHandler = jwtHandler;
            _queries = queries;
            _business = business;
        }

        [HttpGet]
        [AllowAnonymous]
        [DisableCors]
        //[ProducesResponseType(ApiStatusCodes.Ok200, Type =
        //    typeof(SuccessResponse<AutoRegisterResponse>))]
        public async Task<IActionResult> Authorize([FromQuery]string client_1boss)
        {
            //tạo AUTHORIZATION_CODE JWT có caim là client_1boss
            var claims = new JwtCustomClaims
            {
                domain = client_1boss
                // client = "110511085168425756047"
            };
            //Lưu thông tin agentUserId
            var jwt = await _jwtHandler.CreateToken(claims);
            //return redirect_uri
            return ASOFTSuccess(jwt.Token);
        }

        [HttpGet]
        [AllowAnonymous]
        //[ProducesResponseType(ApiStatusCodes.Ok200, Type =
        //typeof(SuccessResponse<IOTHost>))]
        public async Task<string> GetHost(CancellationToken cancellationToken = default)
        {
            var list = await _queries.GetHost(cancellationToken);
            return list;
        }

        [HttpPost]
        [AllowAnonymous]
        [DisableCors]
        //[ProducesResponseType(ApiStatusCodes.Ok200, Type =
        //    typeof(SuccessResponse<AutoRegisterResponse>))]
        public async Task<object> Token(
            [FromForm] string client_id,
            [FromForm] string client_secret,
            [FromForm] string grant_type,
            [FromForm] string refresh_token,
            [FromForm] string code,
            [FromForm] string redirect_uri,
            [FromForm] string remote
            )
        {
            if (grant_type == "authorization_code")
            {
                if (_jwtHandler.ValidateToken(code))
                {
                    var claims = _jwtHandler.getClaims(code);
                    var refreshToken = Guid.NewGuid().ToString();
                    var token = await _business.UpdateAccessToken(claims.domain, refreshToken, claims.host);
                    return Ok(new TokenResponse
                    {
                        access_token = token,
                        expires_in = new DateTimeOffset(DateTime.Now.AddDays(365)).ToUnixTimeSeconds(),
                        refresh_token = refreshToken,
                        token_type = "Bearer"
                    });
                }
            }
            if (grant_type == "refresh_token")
            {
                var token = await _business.RenewAccessToken(refresh_token);
                var orginToken = GetRefToken();
                if (!string.IsNullOrEmpty(token))
                {
                    return Ok(new TokenResponse
                    {
                      //  refresh_token = refreshToken.Token,
                        access_token = token,
                        expires_in = new DateTimeOffset(DateTime.Now.AddDays(365)).ToUnixTimeSeconds(),
                        token_type = "Bearer"
                    });
                }
            }
            return BadRequest(new ErrorResponse() { Error = "invalid_grant" });
        }

        private void SaveRefToken(string token)
        {

            System.IO.File.WriteAllText("tokens.txt", token);

        }

        private string GetRefToken()
        {
            if (System.IO.File.Exists("tokens.txt"))
            {
                return System.IO.File.ReadAllText("tokens.txt");
                //System.IO.File.WriteAllText("tokens.txt", token);
            }
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        //[ProducesResponseType(ApiStatusCodes.Ok200, Type =
        //typeof(SuccessResponse<IOTHost>))]
        public async Task<object> Test(CancellationToken cancellationToken = default)
        {
            //var token = await _jwtHandler.CreateToken(new JwtCustomClaims
            //{
            //    domain = "1boss.vn",
            //    host = "192.168.0.198:8123"
            //}, 3650);

            //if (_jwtHandler.ValidateToken(token.Token))
            //{
            //    var cliamm = _jwtHandler.getClaims(token.Token);
            //    return token.Token;
            //}

            //return "False";
            return BadRequest(new ErrorResponse() { Error = "invalid_grant" });
        }

        //private async Task<object> CallIoTToken(OAuthTokenRequest request, JwtCustomClaims claims)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        var tokenUrl = $"http://{claims.host}/auth/token";
        //        var json = JsonConvert.SerializeObject(request);
        //        //var data = new StringContent(json, Encoding.UTF8, "application/json");
        //        var formData = new MultipartFormDataContent();
        //        formData.Add(new StringContent(request.client_id), "client_id");
        //        formData.Add(new StringContent(request.client_secret), "client_secret");
        //        formData.Add(new StringContent(request.code), "code");
        //        formData.Add(new StringContent(request.grant_type), "grant_type");
        //        formData.Add(new StringContent(request.redirect_uri), "redirect_uri");
        //        formData.Add(new StringContent(request.refresh_token ?? ""), "refresh_token");
        //        formData.Add(new StringContent(request.remote ?? ""), "remote");
        //        formData.Add(new StringContent("skip"), "skip");
        //        using (var tokenResponse = await httpClient.PostAsync(tokenUrl, formData))
        //        {
        //            string tokenApiResponse = await tokenResponse.Content.ReadAsStringAsync();
        //            var result = JsonConvert.DeserializeObject(tokenApiResponse);
        //            return Ok(result);
        //        }
        //    }
        //    //return null;
        //}

        [HttpPost]
        [AllowAnonymous]
        [DisableCors]
        //[ProducesResponseType(ApiStatusCodes.Ok200, Type =
        //    typeof(SuccessResponse<AutoRegisterResponse>))]
        public async Task<object> GoogleAssistant([FromBody]dynamic request)
        {
            //Lấy thông tin hosting dựa trên 
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var shouldReportState = false;

            try
            {
                if (request["inputs"][0]["intent"] == "action.devices.EXECUTE") shouldReportState = true;
            }
            catch (Exception)
            {

            }
            var account = await _business.ValidateAccessToken(accessToken);
            if(account != null)
            {
                using (var httpClient = new HttpClient())
                {
                    var url = $"{account.Host}/api/google_assistant";
                    request["userID"] = account.Domain;
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI1YTA2MDBhODAwNmI0NDgzOThlN2NhMjY3MGZkYzhjYSIsImlhdCI6MTY3MzE4MzAxNiwiZXhwIjoxOTg4NTQzMDE2fQ.TAPeH6nrcYeR5-Qsuof2dNHwsuUXs5NHFQij26kA42A");
                    var json = JsonConvert.SerializeObject(request);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    using (var tokenResponse = await httpClient.PostAsync(url, data))
                    {
                        string tokenApiResponse = await tokenResponse.Content.ReadAsStringAsync();
                        if (shouldReportState)
                        {
                            var shouldReportStateResult = JsonConvert.DeserializeObject<AssisatantResult<ExecuteResult>>(tokenApiResponse);
                            _ReportStateAndNotificationAsync(shouldReportStateResult.data, shouldReportStateResult.access_token, account.Domain).Forget();
                            return Ok(shouldReportStateResult.data);
                        }
                        var result = JsonConvert.DeserializeObject<AssisatantResult<object>>(tokenApiResponse);
                        return Ok(result.data);
                    }
                }
            }
            //if (_jwtHandler.ValidateToken(accessToken))
            //{
                
            //}
            return Unauthorized();
        }

        private async Task _ReportStateAndNotificationAsync(ExecuteResult executeResult, string accessToken, string userID)
        {
            try
            {
                var request = new SyncResult
                {
                    requestId = Guid.NewGuid().ToString(),
                    agentUserId = userID,
                    payload = new SyncPayload
                    {
                        devices = new SyncPayloadDevice
                        {
                            states = new Dictionary<string, object>()
                        }
                    }
                };
                foreach (var command in executeResult.payload.commands)
                {
                    try
                    {
                        request.payload.devices.states.Add(command.ids[0], command.states);
                    }
                    catch (Exception e)
                    {

                        continue;
                    }
                }
                using (var httpClient = new HttpClient())
                {
                    var url = $"https://homegraph.googleapis.com/v1/devices:reportStateAndNotification";
                    var json = JsonConvert.SerializeObject(request);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var result = await httpClient.PostAsync(url, data);
                }
            }
            catch (Exception e)
            {

            }
        }

        [HttpPost]
        [AllowAnonymous]
        [DisableCors]
        public async Task<object> RequestSync([FromBody] object request)
        {
            //Lấy thông tin hosting dựa trên 
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString();
            try
            {
                object result;
                using (var httpClient = new HttpClient())
                {
                    var url = $"https://homegraph.googleapis.com/v1/devices:requestSync";
                    var json = JsonConvert.SerializeObject(request);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, accessToken);
                    using (var apiResponse = await httpClient.PostAsync(url, data))
                    {
                        string tokenApiResponse = await apiResponse.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject(tokenApiResponse);
                        return Ok(result);
                    }
                }
            }
            catch { return BadRequest(); }
        }

        [HttpPost]
        [AllowAnonymous]
        [DisableCors]
        public async Task<object> ReportStateAndNotification([FromBody] object request)
        {
            //Lấy thông tin hosting dựa trên 
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = $"https://homegraph.googleapis.com/v1/devices:reportStateAndNotification";
                    var json = JsonConvert.SerializeObject(request);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, accessToken);
                    using (var apiResponse = await httpClient.PostAsync(url, data))
                    {
                        string tokenApiResponse = await apiResponse.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject(tokenApiResponse);
                        return Ok(result);
                    }
                }
            }
            catch { return BadRequest(); }

        }
    }

    public class AssisatantResult<T> where T : class
    {
        public string access_token { get; set; }
        public T data { get; set; }
    }

    public class GoogleAssitantCommand
    {
        public List<string> ids { get; set; }
        public string status { get; set; }
        public object states { get; set; }
    }

    public class ExecutePayload
    {
        public List<GoogleAssitantCommand> commands { get; set; }
    }

    public class ExecuteResult
    {
        public string requestId { get; set; }
        public ExecutePayload payload { get; set; }
    }

    public class SyncPayload
    {
        public SyncPayloadDevice devices { get; set; }
    }

    public class SyncPayloadDevice
    {
        public Dictionary<string, object> states { get; set; }
    }

    public class SyncResult
    {
        public string requestId { get; set; }
        public string agentUserId { get; set; }
        public SyncPayload payload { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string ha_auth_provider { get; set; }
        public long expires_in { get; set; }
    }

    public class JwtCustomClaims
    {
        public string domain { get; set; }
        public string host { get; set; }
    }

    public class JwtResponse
    {
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
    }

    public interface IJwtHandler
    {
        Task<JwtResponse> CreateToken(JwtCustomClaims claims, int addedDate = 30);
        bool ValidateToken(string token);
        JwtCustomClaims getClaims(string token);
    }

    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
         Convert.FromBase64String(value);
    }

    public class JwtHandler : IJwtHandler
    {
        private readonly ExternalClientJsonConfiguration _settings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public JwtHandler()
        {
            _settings = new ExternalClientJsonConfiguration
            {
                Audience = "Google",
                Issuer = "1BOSS",
                ReferralId = "8c51b38a-f773-4810-8ac5-63b5fb9ca217",
                RsaPrivateKey = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDJAW3KDU2RrEdB\nI7zzokoOnfZj9GW/iP2PxTSP0Zq9vz9rXpOV30ZV+geUn3iZHwm1oL8U8++v0fkP\ncwxgUPQ+Tpyjrn3RXRhnUtcSgt4QSBGcJN4aKwMK6UL5DV7GPoidDlW2+1dLQBDg\nqYa16komNItE/9OCzPj3++tgmnFp7EB9IgyOYOCysVC2eEFlD0QCI+N37OazjM3U\nRihs6k0QvhnxeranMGbd0yqix4H098RpyMsvPj/MyJKu5Hvhtfyum/Y0ZzDmoSkL\nlIFKa3j8TzhkCeZevm8QzADDMMBHegAK1LL+yErxh4PFLJlREMYdvWT6FCoz47R3\n13H01zoTAgMBAAECggEACfzksDQOBS2K0t1MP8zyIhzOR/Q2dBiCBPlXHchcCI6u\nv46RBdL040PLyaJM80rcCCbnJ8rhP5rO5eohFM4g/NGaLfX3MYaPNl8i6bwP/b4f\nBeO9F47IlWu21LpJ2c70qd0y/CcsFMSnsgoqc/Ki4ZgdzX1qzKaTvnrJnOC/vvQU\nMVe9iEOKhZM30kQFxAs1wSt1vruh7bwNGLe2fmoO3HoxYn4DqS6zPPWSxdzQUDHB\nxFLD8BJ12pzHC5A8xcPfwRMSEhRXdx1hX+mTvA8h0n+KFmG/sVhf4egcgzCIQotk\nkJVv7gMsWtirWBuY1+7PW6owc+1cyf4EM0AdXId7BQKBgQD5TLyos6EdLjAOK5S1\nOJDSiEV4AIWiU5GCHgyhtzLcNMHBDOPI7gOqTkiRad8wZO3285BT2c1lMOcMX3s8\n1euYxmPLyx4sH53rlRtdEdyaogZZM/gWRomJcggt1b8H+NN7y5R9f0CMl5LYMzx6\nWI2MAihn+tEiVnJvS2I/mqtALwKBgQDOaGmQ+jo4Au74m/W+ZtNwTms4eKu5trtH\nkEUk6BBAMhnWa/emDZNe69jCiba9au2La+FsENIQQYGQ+YFaj2AlBYQh/Ozr0k48\nnUWjmdnVndLAJGiYO7vlKmtuW+4MfqGOMX5ZyBS+n/9EkQneyB1mzPdcqKVbevT4\nMQgnLZJnXQKBgQD0P4OjQYK7hTId4ALtXkM9kbeGL/S9qmSod+pmUofpCWM/UfKm\nPNOoetRMa874qhofVlh0XrMGzlfAXpfubaRK5SyHUTT5g+yEEG1jzQk3E0FnCiyp\nHmjIGcn7pixokhgZdMbCAffrBuIfImvBbpsXrlGKNknJ+6pYciILMO1RjwKBgBeO\nFNYFqFUyCIF4kt/KrkB3woasRrSn5NFy5mEi1o/s58PviFB75iCD+7Wfr6oIJNmv\nUwdZGA/g2d9oidmC2S8AZSbXzVXQJzuFmyfja9eT7jQdM46D6pppb98lQJJOdTNb\n9JZcXfmvGpuBjcXNwoUzyNrbKBu7eW9FcS+/ZKDFAoGBAO1Ap93xUX9b8xaX91Ug\nFWnXOghABDVDx+UjT1EfraN4kpPx7zAwFFuoVSBt3qYVw640Rd5f26zkWr/zjqt/\nA02EMNh9CXi5D/D85xjFdhwrdxnAh4EKFnUynJxutptbKClS4RMVKDLXL9ZXKI2J\nQr9UNqkXzm+1/nuirwGU0wCD",
                RsaPublicKey = "MIIBCgKCAQEAn4XOc6lV0LZ5j+dBCRH2eiDj6fGlzMIJ7gmSUBF++xLLLAP/EspquIMpTSRJFgrg29euExYNVA+DKDn45ckAXnWar/1JLQdWfz+8ybdUH8mAt9omZStvjfVbqS1/kyBBOymo2LZ3BZCuVRR/kiZ3xuwY06VhgKOcCJR8YQjW5hX+U9Ovl0fLlE4C1a32GBGkcNU7GTrS4aBlciAtALmRLbU+0rr+XJECYWb7/SFfYaM0qAa9kw6FYCfatXclHm2qLaOo8mwlsAdQPpCVyW7R/RrdLgLLkkmzeJacLgjFTLyb894t0Y9/4fHy+L+FAmC+Rceka9ZpCb+/V6IcAZDj+QIDAQAB",
                ReferralUrl = "https://1boss.vn"
            };

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _settings.Issuer,
                ValidateIssuer = true,

                ValidAudience = _settings.Audience,
                ValidateAudience = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("asdfs3402394sdlfjsd123123fljsadfsafsdf234234234234234231423aesr43sf")),
                ValidateIssuerSigningKey = true,

                ValidateLifetime = true
            };
        }

        public async Task<JwtResponse> CreateToken(JwtCustomClaims claims, int addedDate = 30)
        {
            var signingCredentials = new SigningCredentials(_tokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.Now;
            var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();
            var expUnixTimeSeconds = new DateTimeOffset(now.AddDays(addedDate)).ToUnixTimeSeconds();

            if (string.IsNullOrEmpty(claims.host))
            {
                using (var httpClient = new HttpClient())
                {
                    string url = $"https://{claims.domain}/api/v2/a00/OAuth/GetHost";
                    httpClient.DefaultRequestHeaders.Add("api-key", MWConstants.API_KEY);
                    using (var response = await httpClient.GetAsync(url))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            claims.host = JsonConvert.DeserializeObject<string>(apiResponse);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            var jwt = new JwtSecurityToken(
                audience: _settings.Audience,
                issuer: _settings.Issuer,
                claims: new Claim[] {
                    new Claim("domain", claims.domain),
                    new Claim("host", claims.host),
                },
                notBefore: now,
                expires: now.AddDays(addedDate),
                signingCredentials: signingCredentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtResponse
            {
                ExpiresAt = expUnixTimeSeconds,
                Token = token
            };

        }

        public class ExternalClientJsonConfiguration
        {
            public string ReferralUrl { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public string ReferralId { get; set; }
            public string RsaPrivateKey { get; set; }
            public string RsaPublicKey { get; set; }
        }

        public JwtCustomClaims getClaims(string token)
        {
            if (ValidateToken(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                var claims = new JwtCustomClaims
                {
                    domain = jwtToken.Claims.First(claim => claim.Type == "domain").Value,
                    host = jwtToken.Claims.First(claim => claim.Type == "host").Value,
                };

                return claims;
            }
            return null;
        }

        public bool ValidateToken(string token)
        {
            //var publicKey = _settings.RsaPublicKey.ToByteArray();

            //var signingCredentials = new SigningCredentials(_tokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.RsaSha256)
            //{
            //    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            //};
            //var validationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidateAudience = true,
            //    ValidateLifetime = true,
            //    ValidateIssuerSigningKey = true,
            //    ValidIssuer = _settings.Issuer,
            //    ValidAudience = _settings.Audience,
            //    IssuerSigningKey = new RsaSecurityKey(rsa),

            //    CryptoProviderFactory = new CryptoProviderFactory()
            //    {
            //        CacheSignatureProviders = false
            //    }
            //};

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, _tokenValidationParameters, out var validatedSecurityToken);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
