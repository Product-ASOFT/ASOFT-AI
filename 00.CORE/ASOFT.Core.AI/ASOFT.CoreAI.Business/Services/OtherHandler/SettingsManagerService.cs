// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.
// #
// # History：
// #	Date Time	    Updated		    Content
// #    10/07/2025      Đức Mạnh        Tạo mới
// ##################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.DataAccess.Enums;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class SettingsManagerService
    {
        private IASOFTCommonQueries _aSOFCommonQueries;
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly ICIF1640DAL _cif1640DAL;
        private readonly IONT1030Service _ONT1030Queries;

        private readonly ConfigManagerService _configManagerService;

        public SettingsManagerService(IASOFTCommonQueries aSOFTCommonQueries,
            IRedisMemoryProvider vectorDatabase, ICIF1640DAL cif1640DAL,
            IONT1030Service ONT1030Queries, ConfigManagerService configManagerService)
        {
            _aSOFCommonQueries = aSOFTCommonQueries;
            _vectorDatabase = vectorDatabase;
            _cif1640DAL = cif1640DAL;
            _ONT1030Queries = ONT1030Queries;
            _configManagerService = configManagerService;
        }

        #region Lấy các cấu hình model AI từ bảng ONT1030

        /// <summary>
        /// Kiểm tra và lấy cấu hình Model AI
        /// </summary>
        /// <returns></returns>
        public async Task<ChatResponseModel> CheckConfigModelAI()
        {
            string cacheKey = AIConstants.ModelAIKey;
            var cachedKey = await _vectorDatabase.IsCheckExistKeyAsync(cacheKey);
            if (cachedKey == false)
            {
                var modelAIConfig = await GetModelConfigAI();
                if (!string.IsNullOrEmpty(modelAIConfig.ModelName) && !string.IsNullOrEmpty(modelAIConfig.ApiKey))
                {
                    double day = 1;
                    string apiKey = await _vectorDatabase.SaveAPIKeyAsync(cacheKey, modelAIConfig, day);
                    return ChatHandlerHelper.CreateResponse(Guid.Empty, apiKey, true);
                }
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Không có thông tin cấu hình Model AI", false);
            }
            return ChatHandlerHelper.CreateResponse(Guid.Empty, cachedKey.ToString(), true);
        }

        /// <summary>
        /// Lấy cấu hình Model AI từ bảng ONT1030, nếu không có thì lấy từ CIF1640
        /// </summary>
        /// <returns></returns>
        public async Task<ModelAIChatConfig> GetModelConfigAI()
        {
            string apiKey = string.Empty;
            string modelName = string.Empty;
            string modelEmbedding = string.Empty;

            // 1. Lấy danh sách model AI từ ONT1030
            var modelAIs = await _ONT1030Queries.GetAIModelsAsync();

            if (modelAIs != null && modelAIs.Any())
            {
                var modelAI = modelAIs.FirstOrDefault();
                if (modelAI != null)
                {
                    if (!string.IsNullOrEmpty(modelAI.APIKey))
                        apiKey = modelAI.APIKey;

                    if (!string.IsNullOrEmpty(modelAI.ModelName))
                        modelName = modelAI.ModelName;
                    if (!string.IsNullOrEmpty(modelAI.ModelEmbedding))
                        modelEmbedding = modelAI.ModelEmbedding;
                }
            }
            else
            {
                // 2. Fallback lấy từ CIF1640
                var modelAI = await _cif1640DAL.GetConfigModelAI();

                if (modelAI != null)
                {
                    if (!string.IsNullOrEmpty(modelAI.APIKey))
                        apiKey = modelAI.APIKey;

                    if (!string.IsNullOrEmpty(modelAI.ChatbotModel))
                        modelName = modelAI.ChatbotModel;
                }
                modelEmbedding = _configManagerService.GetConfigStringAsync(APIConfigKeys.AI_MODEL_EMBEDDING).Result;
            }

            // 3. Trả về model cấu hình cuối cùng
            var result = new ModelAIChatConfig
            {
                ApiKey = apiKey,
                ModelName = modelName,
                ModelEmbedding = modelEmbedding
            };

            return result;
        }

        #endregion Lấy các cấu hình model AI từ bảng ONT1030

        #region Lấy các cấu hình từ bảng ONT1021

        // Lấy giá trị cấu hình dạng chuỗi từ bảng ONT1021, nếu không có thì lấy từ appsettings.json

        // Lấy số bản ghi tối đa cho lịch sử chat và dữ liệu huấn luyện AI
        public async Task<(int maxChat, int maxTraining)> GetNumberRecordsAsync()
        {
            int maxChatRecords = await _configManagerService.GetConfigIntAsync(APIConfigKeys.CHAT_HISTORY_MAX_RECORDS);

            int maxTrainingRecords = await _configManagerService.GetConfigIntAsync(APIConfigKeys.AI_TRAINING_MAX_RECORDS);

            return (maxChatRecords, maxTrainingRecords);
        }

        // Lấy API Key của dịch vụ OCR bên ngoài
        public async Task<string> GetKeyReadOCRAsync()
        {
            return await _configManagerService.GetConfigStringAsync(APIConfigKeys.AI_OCR_EXTERNAL_API_KEY);
        }

        // Lấy cấu hình sử dụng dịch vụ OCR nội bộ hay không
        public async Task<bool> GetIsUseServiceReadOCRAsync()
        {
            return await _configManagerService.GetConfigBoolAsync(APIConfigKeys.OCR_USE_LOCAL_SERVICE);
        }

        // Lấy URL của API OCR
        public async Task<string> GetUrlReadOCRAsync()
        {
            return await _configManagerService.GetConfigStringAsync(APIConfigKeys.AI_OCR_BASEURL);
        }

        // Lấy URL của ERP
        public async Task<string> GetUrlERPAsync()
        {
            var urlERP = await _configManagerService.GetConfigStringAsync(APIConfigKeys.AI_ERP_BASEURL);
            if (string.IsNullOrEmpty(urlERP))
            {
                string API_Domain = (await _aSOFCommonQueries.GetConfigST2101ByKey((int)GroupConfig.HostingNAPI, "MainURL")).KeyValue;
                string API_PORT = (await _aSOFCommonQueries.GetConfigST2101ByKey((int)GroupConfig.HostingNAPI, "MainPort")).KeyValue;
                string strHttp = @"http://";
                if (!API_Domain.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !API_Domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    urlERP = strHttp + API_Domain;
                }
                else
                {
                    urlERP = API_Domain;
                }
                urlERP += ":" + API_PORT;
            }
            return urlERP;
        }

        #endregion Lấy các cấu hình từ bảng ONT1021
    }
}