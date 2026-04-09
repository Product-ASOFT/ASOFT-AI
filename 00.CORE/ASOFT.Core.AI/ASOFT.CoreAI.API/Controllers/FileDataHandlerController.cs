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
        private readonly AgentCompareService _agentCompareService;
        public FileDataHandlerController(ReadFileOrchestratorService orchestrator, FilePathService filePathService, AgentCompareService agentCompareService)
        {
            _orchestrator = orchestrator;
            _filePathService = filePathService;
            _agentCompareService = agentCompareService;
        }
        [HttpPost]
        [ActionName("HandlerFile")]
        public async Task<ChatResponseReadFileModel> HandlerFileAsync([FromBody] ReadFileRequest request)
        {

            return await _orchestrator.HandleAsync(request);
        }
        [HttpPost]
        [ActionName("UploadFile")]
        public async Task<ChatResponseModel> UploadFileAsync([FromForm] List<IFormFile> files, [FromForm] bool IsCompare)
        {
            return await _filePathService.UpLoadFile(files, IsCompare);
        }
        [HttpGet]
        [ActionName("ConvertJSon")]
        public async Task ConvertJSon(Guid apkMaster)
        {
            string json = @"{
  ""sections"": [
    {
      ""master"": {
        ""SectionOrder"": 1,
        ""SectionType"": ""INVOICE"",
        ""SectionTitle"": ""INVOICE"",
        ""TotalAmount"": 71851000,
        ""TotalCurrency"": ""USD"",
        ""Signature"": ""BLANK""
      },
      ""details"": [
        {
          ""OrderNo"": ""1"",
          ""VoucherNo"": ""28484 TT"",
          ""VoucherDate"": ""2025-12-26"",
          ""Amount"": 71851000,
          ""Currency"": ""USD"",
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD."",
          ""DeliveryTerm"": ""FOB YOKOHAMA""
        }
      ]
    },
    {
      ""master"": {
        ""SectionOrder"": 2,
        ""SectionType"": ""PACKINGLIST"",
        ""SectionTitle"": ""PACKING LIST"",
        ""TotalAmount"": 0,
        ""TotalCurrency"": null,
        ""Signature"": ""BLANK""
      },
      ""details"": [
        {
          ""OrderNo"": ""1"",
          ""PackingListNo"": ""28484TT"",
          ""PackingListDate"": ""2025-12-02"",
          ""GoodsName"": ""Clean roller type cleaning machine"",
          ""Quantity"": 1,
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD.""
        },
        {
          ""OrderNo"": ""2"",
          ""PackingListNo"": ""28484TT"",
          ""PackingListDate"": ""2025-12-02"",
          ""GoodsName"": ""MC-2000 Robo Sticky"",
          ""Quantity"": 1,
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD.""
        },
        {
          ""OrderNo"": ""3"",
          ""PackingListNo"": ""28484TT"",
          ""PackingListDate"": ""2025-12-02"",
          ""GoodsName"": ""Cleaning tape for MC-2000"",
          ""Quantity"": 1,
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD.""
        }
      ]
    }
  ]
}";
            var bEMT2003 = new BEMT2003()
            {
                APK = Guid.NewGuid(),
                CreateUserID = "admin",
            };
            await _agentCompareService.ProcessInfomationFileAsync(json, bEMT2003, "");
            string s = "";
        }
    }
}