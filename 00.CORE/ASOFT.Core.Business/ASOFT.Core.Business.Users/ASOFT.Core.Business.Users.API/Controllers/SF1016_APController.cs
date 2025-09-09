using ASOFT.A00.Entities.Requests;
using ASOFT.Core.API.Controllers;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.Common.Security.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.API.Core.Controllers
{
    [Route("api/Core/Common/Users/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class SF1016_APController : ASOFTBaseController
    {
        private readonly IAuthenticationBusiness _authBusiness;

        public SF1016_APController(IAuthenticationBusiness authBusiness)
        {
            _authBusiness = Checker.NotNull(authBusiness, nameof(authBusiness));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([Required][FromBody] ChangePasswordRequest request,[FromServices] IIdentity identity, CancellationToken cancellationToken)
        {
            var result = await _authBusiness.ChangePassword(identity.ID, request.OldPassword, request.NewPassword, request.DivisionID, request.DeviceID, cancellationToken);
            if (result.IsSucceed)
            {
                return ASOFTSuccess(result.Success);
            }
            return ASOFTError(result.Error);
        }
    }
}
