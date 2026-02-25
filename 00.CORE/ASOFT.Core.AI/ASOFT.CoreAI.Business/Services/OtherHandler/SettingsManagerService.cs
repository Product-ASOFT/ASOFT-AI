// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.
// #
// # History：
// #	Date Time	    Updated		    Content
// #    10/07/2025      Đức Mạnh        Tạo mới
// ##################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.DataAccess.Enums;
using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Business.LibraryKernel;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.System;
using ASOFT.CoreAI.Infrastructure;
using HandlebarsDotNet;
using Microsoft.OpenApi.Exceptions;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using System.Threading;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class SettingsManagerService
    {
        private IASOFTCommonQueries _aSOFCommonQueries;
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly ICIF1640DAL _cif1640DAL;
        private readonly IONT1030Service _ONT1030Queries;
        private readonly ConfigManagerService _configManagerService;
        private readonly Kernel _kernel;

        public SettingsManagerService(IASOFTCommonQueries aSOFTCommonQueries,
            IRedisMemoryProvider vectorDatabase, ICIF1640DAL cif1640DAL,
            IONT1030Service ONT1030Queries, ConfigManagerService configManagerService, Kernel kernel)
        {
            _aSOFCommonQueries = aSOFTCommonQueries;
            _vectorDatabase = vectorDatabase;
            _cif1640DAL = cif1640DAL;
            _ONT1030Queries = ONT1030Queries;
            _configManagerService = configManagerService;
            _kernel = kernel;
        }

        #region Lấy các cấu hình model AI từ bảng ONT1030

        /// <summary>
        /// Kiểm tra và lấy cấu hình Model (dùng cho API/Health check)
        /// </summary>
        public async Task<ChatResponseModel> CheckConfigModelAI()
        {
            var (config, hasConfig, _, keyStatus, errorMsg) = await EnsureModelAIConfigCachedAsync();
            if (!hasConfig)
                return ChatHandlerHelper.CreateResponse(Guid.Empty, errorMsg!, false);

            if (keyStatus != AIKeyStatus.Valid)
                return ChatHandlerHelper.CreateResponse(Guid.Empty, errorMsg!, false);

            return ChatHandlerHelper.CreateResponse(Guid.NewGuid(), config.ApiKey, true);
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

        public async Task<(ModelAIChatConfig Config, bool HasConfig, bool CreatedNew, AIKeyStatus KeyStatus, string? ErrorMessage)> EnsureModelAIConfigCachedAsync()
        {
            var configLLM = await GetConfigLLMsAsync();
            if (configLLM != null && configLLM.IsUse)
            {
                return (new ModelAIChatConfig(), true, true, AIKeyStatus.Valid, "");
            }
            else
            {
                string cacheKey = AIConstants.ModelAIKey;

                var cachedKey = await _vectorDatabase.GetApiKeyAsync(cacheKey);
                bool createdNew = false;

                // 1. Nếu cache chưa có → load config từ DB và lưu lại cache
                if (string.IsNullOrWhiteSpace(cachedKey))
                {
                    var configModelAI = await GetModelConfigAI();

                    if (string.IsNullOrEmpty(configModelAI.ModelName) || string.IsNullOrEmpty(configModelAI.ApiKey))
                    {
                        return (new ModelAIChatConfig(), false, false, AIKeyStatus.InvalidKey, "Không có thông tin cấu hình Model AI");
                    }

                    double expireDays = await _configManagerService.GetConfigIntAsync(APIConfigKeys.REDIS_DEFAULT_EXPIRE_DAYS, 1);

                    await _vectorDatabase.SaveAPIKeyAsync(cacheKey, configModelAI, expireDays);

                    cachedKey = await _vectorDatabase.GetApiKeyAsync(cacheKey);
                    createdNew = true;
                }

                if (string.IsNullOrWhiteSpace(cachedKey))
                {
                    return (new ModelAIChatConfig(), false, createdNew, AIKeyStatus.InvalidKey, "Không đọc được dữ liệu Model AI từ cache");
                }

                // Fix trường hợp bị bao bằng {{...}}
                if (cachedKey.StartsWith("{{") && cachedKey.EndsWith("}}"))
                {
                    cachedKey = cachedKey.Substring(1, cachedKey.Length - 2);
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var config = JsonSerializer.Deserialize<ModelAIChatConfig>(cachedKey, options)
                             ?? new ModelAIChatConfig();

                bool hasConfig = !string.IsNullOrEmpty(config.ModelName) && !string.IsNullOrEmpty(config.ApiKey);

                if (!hasConfig)
                {
                    return (config, false, createdNew, AIKeyStatus.InvalidKey, "Cấu hình Model AI không hợp lệ");
                }

                var keyStatus = await CheckKeyWithAsync();
                string errorMsg = "";
                if (keyStatus == AIKeyStatus.InvalidKey)
                {
                    errorMsg = "API Key không hợp lệ. Vui lòng kiểm tra lại API key.";
                }
                else if (keyStatus == AIKeyStatus.OutOfCredit)
                {
                    errorMsg = "API Key của bạn không đủ token để thực hiện câu hỏi. Vui lòng kiểm tra lại API Key";
                }
                else if (keyStatus == AIKeyStatus.UnknownError)
                {
                    errorMsg = "Không thể xác thực API Key truy cập AI vào thời điểm này.Vui lòng kiểm tra lại API Key";
                }
                return (config, true, createdNew, keyStatus, errorMsg);
            }
        }
        private async Task<AIKeyStatus> CheckKeyWithAsync()
        {
            try
            {
                var promptTemplate = "{{test}}";
                var arguments = new KernelArguments
                {
                    ["test"] = "ping"
                };
                var result = await _kernel.InvokePromptAsync(promptTemplate, arguments, "handlebars", new HandlebarsPromptTemplateFactory());
                if (result.Metadata is not null && result.Metadata.TryGetValue(AIConstants.ErrorType, out var errorTypeObj))
                {
                    // Lấy RawErrorMessage để phân loại chi tiết
                    string rawError = string.Empty;
                    if (result.Metadata.TryGetValue(AIConstants.RawErrorMessage, out var rawErrorObj))
                    {
                        rawError = rawErrorObj?.ToString() ?? string.Empty;
                    }

                    var msg = rawError.ToLowerInvariant();

                    if (msg.Contains("invalid_api_key") || msg.Contains("incorrect api key"))
                        return AIKeyStatus.InvalidKey;

                    if (msg.Contains("insufficient_quota") || msg.Contains("billing_hard_limit_reached"))
                        return AIKeyStatus.OutOfCredit;

                    return AIKeyStatus.UnknownError;
                }
                return AIKeyStatus.Valid;
            }
            catch (OpenApiException ex)
            {
                if (ex.Message.Contains("Incorrect API key") ||
                    ex.Message.Contains("invalid_api_key"))
                    return AIKeyStatus.InvalidKey;

                if (ex.Message.Contains("insufficient_quota") ||
                    ex.Message.Contains("billing_hard_limit_reached"))
                    return AIKeyStatus.OutOfCredit;

                return AIKeyStatus.UnknownError;
            }
            catch
            {
                return AIKeyStatus.UnknownError;
            }
        }
        // Lấy URL của API LLMs
        public async Task<LLMViewModel> GetConfigLLMsAsync()
        {
            return new LLMViewModel()
            {
                BaseUrl = await _configManagerService.GetConfigStringAsync(APIConfigKeys.AI_AI_LLM_BASEURL),
                IsUse = await _configManagerService.GetConfigBoolAsync(APIConfigKeys.AI_AI_LLM_ISUSE),
                Temperature = await _configManagerService.GetConfigDoubleAsync(APIConfigKeys.AI_AI_LLM_TEMPERATURE),
                MaxToken = await _configManagerService.GetConfigIntAsync(APIConfigKeys.AI_AI_LLM_MAXTOKEN),
            };
        }
    }
}