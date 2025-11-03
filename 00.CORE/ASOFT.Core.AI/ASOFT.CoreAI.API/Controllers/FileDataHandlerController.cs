using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Entities;
using ASOFT.OO.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ASOFT.CoreAI.API.Controllers
{
    // hàm AgentPromptController sẽ quản lý các prompt của agent
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class FileDataHandlerController : AgentBaseController
    {
        private readonly ReadFileOrchestratorService _orchestrator;
        private readonly FilePathService _filePathService;
        public FileDataHandlerController( ReadFileOrchestratorService orchestrator, FilePathService filePathService)
        {
            _orchestrator = orchestrator;
            _filePathService = filePathService;
        }
        [HttpPost]
        [ActionName("HandlerFile")]
        public async Task<ChatResponseReadFileModel> HandlerFileAsync([FromBody] ReadFileRequest request)
        {
            return await _orchestrator.HandleAsync(request);
        }
        [HttpPost]
        [ActionName("UploadFile")]
        public async Task<ChatResponseModel> UploadFileAsync([FromForm] List<IFormFile> files)
        {
            return await _filePathService.UpLoadFile(files);
        }
        [HttpPost]
        [ActionName("CreateFile")]
        public async Task<ChatResponseReadFileModel> CreateFileAsync(ReadFileRequest request)
        {
            return await _filePathService.CreateFile(request);
        }
    }
}