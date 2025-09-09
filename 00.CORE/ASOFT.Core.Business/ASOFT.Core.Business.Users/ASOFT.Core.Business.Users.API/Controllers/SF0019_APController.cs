using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Business.Users.Entities.Requests;
using ASOFT.Core.Business.Users.Entities;
using ASOFT.Core.Common.InjectionChecker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using ASOFT.A00.Entities.Requests;
using System.ComponentModel.DataAnnotations;
using ASOFT.Core.Common.Security.Identity;
using ASOFT.Core.API.Versions;

namespace ASOFT.API.Core.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [Route("api/v{version:api-version}/Core/Common/Users/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class SF0019_APController : ASOFTBaseController
    {
        private readonly IAuthenticationBusiness _authBusiness;

        public SF0019_APController(IAuthenticationBusiness authBusiness)
        {
            _authBusiness = Checker.NotNull(authBusiness, nameof(authBusiness));
        }

        /// <summary>
        /// Kiểm tra Password khi kích hoạt tính năng đăng nhập sinh trắc học
        /// </summary>
        /// <param name="password"></param>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<AuthenticatedModel>))]
        public async Task<IActionResult> VerifyPassword(
            [FromQuery] string password,
            [FromServices] IIdentity identity,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var signInCommand = new VerifyPasswordRequest 
            {
                Password = password,
                UserID = identity.ID
            };
            var result = await _authBusiness.VerifyPassword(signInCommand, cancellationToken);

            if (result == false)
            {
                return ASOFTError(new ErrorModelV2 { Code = DefaultErrorCodes.InvalidUserNameOrPassword });
            }

            return ASOFTSuccess(result);
        }

        /// <summary>
        /// Lưu Biometrics key
        /// </summary>
        /// <param name="biometricsKey"></param>
        /// <param name="divisionID"></param>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateBiometricsKey(
            [FromBody] UpdateBiometricsKeyRequest uploadRequest,
            [FromServices] IIdentity identity, 
            CancellationToken cancellationToken)
        {
            uploadRequest.UserID = identity.ID;
            var result = await _authBusiness.UpdateBiometricsKey(uploadRequest, cancellationToken);
            if (result.IsSucceed)
            {
                return ASOFTSuccess(result.Success);
            }
            return ASOFTError(result.Error);
        }
    }
}
