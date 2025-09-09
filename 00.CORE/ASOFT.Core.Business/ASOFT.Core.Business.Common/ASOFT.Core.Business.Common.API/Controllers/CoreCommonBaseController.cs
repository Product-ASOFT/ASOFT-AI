using ASOFT.Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.Core.Business.Common.API.Controllers
{
    [Route("api/v{version:api-version}/Core/Common/[controller]/[action]")]
    public class CoreCommonBaseController : ASOFTBaseController
    {
    }
}
