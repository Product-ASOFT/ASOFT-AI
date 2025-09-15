using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.OO.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.CoreAI.API.Controllers
{
    // hàm AgentPromptController sẽ quản lý các prompt của agent
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class RedisDataController : AgentBaseController
    {
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly IDataLoader _dataLoader;
        private readonly IRedisHandler _redisHandler;
        private readonly AgentManager _agentService;
        private readonly ICIF1640DAL _cif1640DAL;

        public RedisDataController(IRedisMemoryProvider vectorDatabase,
            IDataLoader dataLoader,
            IRedisHandler redisHandler,
            AgentManager agentManager,
            ICIF1640DAL cif1640DAL)
        {
            _vectorDatabase = vectorDatabase;
            _dataLoader = dataLoader;
            _redisHandler = redisHandler;
            _agentService = agentManager;
            _cif1640DAL = cif1640DAL;
        }

        [ActionName("TrainingData")]
        [HttpPost]
        public async Task<ChatResponseModel> CreateDataTrainAsync([FromBody] LoadFileRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.FilePath))
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Invalid request or file path is empty.");
            }
            try
            {
                await _dataLoader.LoadTrainingDataFromDocument(request, cancellationToken);
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Create data train successfully.");
            }
            catch (Exception ex)
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, $"Error creating training data: {ex.Message}");
            }
        }

        [HttpGet]
        [ActionName("CheckModelAIConfig")]
        public async Task<ChatResponseModel> CheckModelAIConfigAsync()
        {
            string cacheKey = AIConstants.ModelAIKey;
            var cachedKey = await _vectorDatabase.IsCheckExistKeyAsync(cacheKey);
            if (cachedKey == false)
            {
                var configModelAI = await _cif1640DAL.GetConfigModelAI();
                if (configModelAI != null && !string.IsNullOrEmpty(configModelAI.APIKey) && !string.IsNullOrEmpty(configModelAI.ChatbotModel))
                {
                    double day = 1; // Thời gian lưu trữ key, có thể lấy từ config hoặc tham số
                    var modelAIConfig = new ModelAIChatConfig
                    {
                        ApiKey = configModelAI.APIKey,
                        ModelName = configModelAI.ChatbotModel,
                    };
                    string apiKey = await _vectorDatabase.SaveAPIKeyAsync(cacheKey, modelAIConfig, day);
                    return ChatHandlerHelper.CreateResponse(Guid.Empty, apiKey);
                }
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Không có thông tin cấu hình Model AI");
            }
            return ChatHandlerHelper.CreateResponse(Guid.Empty, cachedKey.ToString());
        }

        [HttpPost]
        [ActionName("GetApiKeyFromExternal")]
        public async Task<ChatResponseModel> GetApiKeyAsync([FromBody] ModelAIChatConfig config)
        {
            string cacheKey = $"ModelAIKey";
            var cachedKey = await _vectorDatabase.IsCheckExistKeyAsync(cacheKey);
            if (cachedKey)
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, cachedKey.ToString());
            }
            // Lưu key vào Redis với TTL (ví dụ 1 tiếng)
            double day = 1; // Thời gian lưu trữ key, có thể lấy từ config hoặc tham số
            string apiKey = await _vectorDatabase.SaveAPIKeyAsync(cacheKey, config, day);

            return ChatHandlerHelper.CreateResponse(Guid.Empty, apiKey);
        }

        [HttpPost]
        [ActionName("GetAnswer")]
        public async Task<ChatResponseModel> GetAnswerAsync([FromBody] AgentRequest agentRequest)
        {
            if (agentRequest == null || string.IsNullOrWhiteSpace(agentRequest.Question))
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Không có thông tin về câu hỏi");
            }
            string indexName = "pdf_content"; // Tên của Index trong Redis Vector Store
            var jsonObjects = await _redisHandler.GetDataTrainFormRedis(agentRequest.Question, indexName, 5);
            if (jsonObjects == null || !jsonObjects.Any())
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Không có thông tin dữ liệu được trainning.");
            }
            // Nếu muốn gán lại cho agentRequest
            agentRequest.Items = jsonObjects;

            // Chuyển đổi kết quả thành chuỗi trả lời
            var responseModel = await _agentService.CallHandlerAgentAsync(agentRequest);
            return responseModel;
        }
    }
}