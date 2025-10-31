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
        private IST2130Queries _agentPromptQueries;

        public AgentPromptController(IST2130Queries agentPromptQueries)
        {
            _agentPromptQueries = agentPromptQueries;
        }

        //   Lấy danh sách các prompt của agent theo mã agent
        [HttpPost]
        [ActionName("QueryPromptsByAgent")]
        public async Task<ST2130> QueryPromptsByAgentAsync([FromBody] AgentPromptRequest agentPromptRequest)
        {
            return await _agentPromptQueries.QueryPromptsByAgentCode(agentPromptRequest.AgentCode);
        }

        // Thêm mới một prompt cho agent
        [HttpPost]
        [ActionName("CreatePrompt")]
        public async Task<bool> CreatePromptAsync([FromBody] ST2130 agentPrompt)
        {
            return await _agentPromptQueries.CreateAgentPrompt(agentPrompt);
        }
    }
}