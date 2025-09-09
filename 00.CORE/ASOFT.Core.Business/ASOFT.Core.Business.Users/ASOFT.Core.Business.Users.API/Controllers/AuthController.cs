using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Business.Users.DataAccsess.Interfaces;
using ASOFT.Core.Business.Users.Entities;
using ASOFT.Core.Business.Users.Entities.Requests;
using ASOFT.Core.Common.Security.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.API.Controllers
{
    [Route("api/Core/Common/Users/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class AuthController : ASOFTBaseController
    {
        private readonly IAuthenticationBusiness _authBusiness;

        public AuthController(ILogger<AuthController> logger, IAuthenticationBusiness authBusiness, IScreenPermissionQueries screenPermissionQueries)
        {
            _authBusiness = authBusiness ?? throw new ArgumentNullException(nameof(authBusiness));
        }

        /// <summary>
        /// Sign in ERPX
        /// </summary>
        /// <param name="signInCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Clone từ SignIn và chỉnh sửa theo luồng đăng nhập ERPX
        /// </history>
        [AllowAnonymous]
        [DisableCors]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<AuthenticatedERPXModel>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorResponse<ErrorModelV2>))]
        public async Task<IActionResult> SignInERPX(
            [FromBody] SignInERPXRequest signInCommand,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var authenticatedModel = await _authBusiness.SignInERPX(signInCommand, cancellationToken);

            if (authenticatedModel == null)
            {
                return ASOFTError(new ErrorModelV2 { Code = DefaultErrorCodes.InvalidUserNameOrPassword });
            }

            return ASOFTSuccess(authenticatedModel);
        }

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="signInCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [DisableCors]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<AuthenticatedModel>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorResponse<ErrorModelV2>))]
        public async Task<IActionResult> SignIn(
            [FromBody] SignInRequest signInCommand,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var authenticatedModel = await _authBusiness.SignIn(signInCommand, cancellationToken);

            if (authenticatedModel == null)
            {
                return ASOFTError(new ErrorModelV2 {Code = DefaultErrorCodes.InvalidUserNameOrPassword });
            }

            return ASOFTSuccess(authenticatedModel);
        }

        /// <summary>
        /// Đăng nhập sinh trắc học
        /// </summary>
        /// <param name="signInCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [DisableCors]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<AuthenticatedModel>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorResponse<ErrorModelV2>))]
        public async Task<IActionResult> SignInBiometrics(
            [FromBody] SignInBiometricsRequest signInCommand,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var authenticatedModel = await _authBusiness.SignInBiometrics(signInCommand, cancellationToken);

            if (authenticatedModel == null)
            {
                return ASOFTError(new ErrorModelV2 { Code = DefaultErrorCodes.InvalidUserNameOrPassword });
            }

            return ASOFTSuccess(authenticatedModel);
        }

        /// <summary>
        /// Sign in dùng để lưu token xuống DB cho ERP9
        /// </summary>
        /// <param name="signInCommand"></param>
        /// <param name="isLoginQR"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<AuthenticatedModel>))]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorResponse<ModelStateErrorModel>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorResponse<ErrorModelV2>))]
        public async Task<IActionResult> SignInERP9(
            [FromBody] SignInRequest signInCommand,
            [FromQuery] bool isLoginQR,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _authBusiness.SignInERP9(signInCommand, isLoginQR, cancellationToken);
            return ASOFTSuccess(null);
        }

        /// <summary>
        /// API lấy thông tin khi app đổi divison
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<AuthenticatedModel>>))]
        public async Task<IActionResult> ChangeDivision(
            [FromQuery] string divisionID,
            [FromServices] IIdentity identity,
            CancellationToken cancellationToken)
        {
            var result = await _authBusiness.ChangeDivison(identity.ID, divisionID, cancellationToken);
            return ASOFTSuccess(result);
        }
    }
}