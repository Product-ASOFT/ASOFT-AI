using ASOFT.Core.API.Https;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Versions;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.Common.Security.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "Core")]
    public class FollowersController : CoreCommonBaseController
    {
        private readonly IFollowerBusiness _followerBusiness;
        public FollowersController(IFollowerBusiness followerBusiness)
        {
            _followerBusiness = Checker.NotNull(followerBusiness, nameof(followerBusiness));
        }

        [HttpGet]
        [ProducesResponseType(ApiStatusCodes.Ok200, Type =
             typeof(SuccessResponse<IEnumerable<FollowerViewModel>>))]
        public async Task<IActionResult> GetFollowerList([Required][FromQuery]Guid apk, [Required][FromQuery]string tableID, CancellationToken cancellationToken = default)
        {
            var followers = await _followerBusiness.GetFollowersAsync(apk.ToString(), tableID, cancellationToken);
            return ASOFTSuccess(followers);
        }


        [HttpPost]
        public async Task<IActionResult> InsertFollower([Required][FromBody]InsertFollowerRequest request, [FromServices] IIdentity viewer, CancellationToken cancellationToken = default)
        {
            request.UserID = viewer.ID;
            var result = await _followerBusiness.SaveFollowerWithTransaction(request, cancellationToken);
            if (!result.IsSucceed)
            {
                return ASOFTForbidden(result.Error);
            }
            return ASOFTSuccess(result.Success);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFollower([Required][FromBody]RemoveFollowerRequest request, [FromServices] IIdentity viewer, CancellationToken cancellationToken = default)
        {
            request.UserID = viewer.ID;
            var result = await _followerBusiness.DeleteFollowerWithTransaction(request, cancellationToken);
            if (!result.IsSucceed)
            {
                return ASOFTForbidden(result.Error);
            }
            return ASOFTSuccess(result.Success);
        }
    }
}
