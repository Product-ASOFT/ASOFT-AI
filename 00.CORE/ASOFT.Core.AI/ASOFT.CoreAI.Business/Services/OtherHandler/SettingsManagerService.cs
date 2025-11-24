using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.DataAccess.Enums;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.AI;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

namespace ASOFT.CoreAI.Business
{
    public class SettingsManagerService
    {
        private readonly IConfiguration _configuration;
        private IASOFTCommonQueries _aSOFCommonQueries;
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly ICIF1640DAL _cif1640DAL;
        public SettingsManagerService(IConfiguration configuration, IASOFTCommonQueries aSOFTCommonQueries,
            IRedisMemoryProvider vectorDatabase,
            ICIF1640DAL cif1640DAL)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _aSOFCommonQueries = aSOFTCommonQueries ?? throw new ArgumentNullException(nameof(aSOFTCommonQueries));
            _vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(nameof(vectorDatabase));
            _cif1640DAL = cif1640DAL ?? throw new ArgumentNullException(nameof(cif1640DAL));
        }
        public (int maxChat, int maxTraining) GetNumberRecords()
        {
            int maxChatRecords = _configuration.GetValue<int>("ChatHistorySettings:MaxRecords");
            int maxTrainingRecords = _configuration.GetValue<int>("TrainingDataSettings:MaxRecords");
            return (maxChatRecords, maxTrainingRecords);
        }

        public async Task<string> GetExternalApi()
        {
            string API_Domain = (await _aSOFCommonQueries.GetConfigST2101ByKey((int)GroupConfig.HostingNAPI, "MainURL")).KeyValue;
            string API_PORT = (await _aSOFCommonQueries.GetConfigST2101ByKey((int)GroupConfig.HostingNAPI, "MainPort")).KeyValue;
            string strHttp = @"http://";
            string newUrl = string.Empty;
            if (!API_Domain.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !API_Domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                newUrl = strHttp + API_Domain;
            }
            else
            {
                newUrl = API_Domain;
            }
            newUrl += ":" + API_PORT;
            return newUrl;
        }

        public string GetKeyReadOCR()
        {
            var apiConfig = _configuration.GetValue<string>("ReadConfigOCR:key-ocr");
            return apiConfig ?? string.Empty;
        }
        public string GetUrlReadOCR()
        {
            var apiConfig = _configuration.GetValue<string>("APIOCR:URL");
            return apiConfig ?? string.Empty;
        }
        public bool GetIsUseServiceReadOCR()
        {
            bool IsUseServiceLocal = false;
            var configUse = _configuration.GetValue<bool>("IsUseServiceReadOCR:Local").ToString();
            if (!string.IsNullOrEmpty(configUse))
            {
                IsUseServiceLocal = bool.Parse(configUse);
            }
            return IsUseServiceLocal;
        }
        public async Task<ChatResponseModel> CheckConfigModelAI()
        {
            string cacheKey = AIConstants.ModelAIKey;
            var cachedKey = await _vectorDatabase.IsCheckExistKeyAsync(cacheKey);
            if (cachedKey == false)
            {
                var configModelAI = await _cif1640DAL.GetConfigModelAI();
                if (configModelAI != null && !string.IsNullOrEmpty(configModelAI.APIKey) && !string.IsNullOrEmpty(configModelAI.ChatbotModel))
                {
                    double day = 1; 
                    var modelAIConfig = new ModelAIChatConfig
                    {
                        ApiKey = configModelAI.APIKey,
                        ModelName = configModelAI.ChatbotModel,
                    };
                    string apiKey = await _vectorDatabase.SaveAPIKeyAsync(cacheKey, modelAIConfig, day);
                    return ChatHandlerHelper.CreateResponse(Guid.Empty, apiKey, true);
                }
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Không có thông tin cấu hình Model AI", false);
            }
            return ChatHandlerHelper.CreateResponse(Guid.Empty, cachedKey.ToString(), true);
        }
    }
}