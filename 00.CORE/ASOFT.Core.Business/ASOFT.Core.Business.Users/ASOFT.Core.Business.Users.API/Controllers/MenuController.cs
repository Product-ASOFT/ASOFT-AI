using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Https;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.Common.Security.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.API.Controllers
{
    [Route("api/Core/Common/Users/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class MenuController : ASOFTBaseController
    {
        private readonly IMenuBusiness _menuBusiness;

        public MenuController(IMenuBusiness menuBusiness)
        {
            _menuBusiness = Checker.NotNull(menuBusiness, nameof(menuBusiness));
        }

        [HttpGet]
        [ProducesResponseType(ApiStatusCodes.Ok200, Type =
            typeof(SuccessResponse<Dictionary<string, AppMenu>>))]
        public async Task<IActionResult> GetMenu([FromQuery][Required]string divisionID,[FromServices] IIdentity identity, CancellationToken cancellationToken = default)
        {
            var result = await _menuBusiness.GetMenu(identity.ID, divisionID, cancellationToken);

            return ASOFTSuccess(result);
        }
    }
}
