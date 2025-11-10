using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entitiess;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.OO.API.Controllers;
using DocumentFormat.OpenXml.Drawing.Diagrams;
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
            return await _agentPromptQueries.GetPromptByCode(agentPromptRequest.AgentCode);
        }

        // Thêm mới một prompt cho agent
        [HttpPost]
        [ActionName("CreatePrompt")]
        public async Task<bool> CreatePromptAsync([FromBody] ST2130 agentPrompt)
        {
            return await _agentPromptQueries.CreateAgentPrompt(agentPrompt);
        }
        [HttpGet]
        [ActionName("CreatePromptAuto")]
        public async Task<bool> CreatePromptAuto()
        {
            var agentPrompt = AddListAgentPrompt();
            return await _agentPromptQueries.CreateListAgentPrompt(agentPrompt);
        }
        private IEnumerable<ST2130> AddListAgentPrompt()
        {
            var agentPrompts = new List<ST2130>();
            string folderPath = @"E:\Works\Architecture\OCR\Prompt";

            // Lấy danh sách file .txt trong thư mục
            string[] files = Directory.GetFiles(folderPath, "*.txt", SearchOption.TopDirectoryOnly);

            // Dictionary lưu tên file và nội dung
            Dictionary<string, string> fileContents = new Dictionary<string, string>();

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string content = System.IO.File.ReadAllText(filePath);
                fileContents[fileName] = content;

                string typeCompare = string.Empty, description = string.Empty;
                if (fileName == "1.PromptCompareWarseHouse.txt")
                {
                    typeCompare = AIConstants.AgentTypeKeys.BEM_AGENT_BEMF2000_WAREHOUSE;
                    description = "Prompt xử lý đối chiếu loại nhập kho";
                }
                else if (fileName == "2.PromptCompareMachine.txt")
                {
                    typeCompare = AIConstants.AgentTypeKeys.BEM_AGENT_BEMF2000_MACHINE;
                    description = "Prompt xử lý đối chiếu loại máy móc";
                }
                else if (fileName == "3.PromptCompareService.txt")
                {
                    typeCompare = AIConstants.AgentTypeKeys.BEM_AGENT_BEMF2000_SERVICE;
                    description = "Prompt xử lý đối chiếu loại dịch vụ";
                }
                else if (fileName == "4.PromptCompareBuild.txt")
                {
                    typeCompare = AIConstants.AgentTypeKeys.BEM_AGENT_BEMF2000_BUILD;
                    description = "Prompt xử lý đối chiếu loại xây dựng";
                }
                else if (fileName == "5.PromptCompareOther.txt")
                {
                    typeCompare = AIConstants.AgentTypeKeys.BEM_AGENT_BEMF2000_OTHER;
                    description = "Prompt xử lý đối chiếu các loại khác";
                }
                agentPrompts.Add(new ST2130
                {
                    APK = Guid.NewGuid(),
                    DivisionID = "ASOFT",
                    AgentCode = "BEM_AGENT_BEMF2000",
                    ModuleCode = "BEM",
                    TypePrompt = "Plugin",
                    PromptContent = content,
                    TypeCompare = typeCompare,
                    Description = description,
                    IsActive = true,
                    Version = 1,
                    CreateDate = DateTime.Now,
                    CreateUserID = "ADMIN"
                });
            }
            return agentPrompts;
        }
    }
}