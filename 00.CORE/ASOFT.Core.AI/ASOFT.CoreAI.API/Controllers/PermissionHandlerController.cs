using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.OO.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class PermissionHandlerController : AgentBaseController
    {
        private IPermissionHandler _permissionHandler;
        private readonly AgentManager _agentService;

        public PermissionHandlerController(IPermissionHandler permissionHandler, AgentManager agentService)
        {
            _permissionHandler = permissionHandler;
            _agentService = agentService;
        }

        // Kiểm tra quyền truy cập của người dùng vào các tính năng Chat Nornal hoặc Chat AIPlugin
        [HttpPost]
        [ActionName("GetAccessType")]
        public async Task<ChatResponseModel> GetAccessTypeAsync([FromBody] AgentRequest request)
        {
            var result = await _permissionHandler.GetAccessType(request.Permisions);
            return ChatHandlerHelper.CreateResponse(request.ChatSessionID, result);
        }

        // Kiểm tra quyền truy cập của người dùng vào các plugin
        [HttpPost]
        [ActionName("GetPluginsUserHasAccess")]
        public async Task<ChatResponseModel> GetPluginsUserHasAccessAsync([FromBody] AgentRequest request)
        {
            var result = await _agentService.CallHandlerAgentAsync(request);
            return result;
        }
    }
}