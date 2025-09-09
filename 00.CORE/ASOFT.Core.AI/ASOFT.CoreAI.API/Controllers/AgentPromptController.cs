using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entitiess;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.OO.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.CoreAI.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class AgentPromptController : AgentBaseController
    {
        private IST2111Queries _agentPromptQueries;

        public AgentPromptController(IST2111Queries agentPromptQueries)
        {
            _agentPromptQueries = agentPromptQueries;
        }

        //   Lấy danh sách các prompt của agent theo mã agent
        [HttpPost]
        [ActionName("QueryPromptsByAgent")]
        public async Task<ST2111> QueryPromptsByAgentAsync([FromBody] AgentPromptRequest agentPromptRequest)
        {
            return await _agentPromptQueries.QueryPromptsByAgentCode(agentPromptRequest.AgentCode);
        }

        // Thêm mới một prompt cho agent
        [HttpPost]
        [ActionName("CreatePrompt")]
        public async Task<bool> CreatePromptAsync([FromBody] ST2111 agentPrompt)
        {
            return await _agentPromptQueries.CreateAgentPrompt(agentPrompt);
        }
    }
}