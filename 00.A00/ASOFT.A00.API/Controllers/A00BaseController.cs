using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Versions;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.A00.API.Controllers
{
    [Route("api/v{version:api-version}/A00/[controller]/[action]")]
    public abstract class A00BaseController : ASOFTBaseController
    {
    }
}