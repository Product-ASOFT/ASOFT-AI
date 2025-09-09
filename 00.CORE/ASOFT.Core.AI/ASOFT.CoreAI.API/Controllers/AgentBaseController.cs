using ASOFT.Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.OO.API.Controllers
{
    [Route("api/v{version:api-version}/CoreAI/[controller]/[action]")]
    public abstract class AgentBaseController : ASOFTBaseController
    {
    }
}