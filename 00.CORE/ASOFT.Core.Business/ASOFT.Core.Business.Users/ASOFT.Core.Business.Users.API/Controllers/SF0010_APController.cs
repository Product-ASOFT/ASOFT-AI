// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    19/02/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.Common.Security.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.API.Core.Controllers
{
    [Route("api/Core/Common/Users/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class SF0010_APController : ASOFTBaseController
    {
        private readonly IUserInfoQueries _userInfoQueries;

        public SF0010_APController(IUserInfoQueries userInfoQueries)
        {
            _userInfoQueries = Checker.NotNull(userInfoQueries, nameof(userInfoQueries));
        }

        /// <summary>
        /// API lấy thông tin người dùng
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [19/02/2020]
        /// </history>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<AP1103ViewModel>))]
        public async Task<IActionResult> GetUserInfo([FromServices] IIdentity identity, CancellationToken cancellationToken)
        {
            var result = await _userInfoQueries.GetUserInfo(identity.ID, cancellationToken);
            return ASOFTSuccess(result);
        }
    }
}
