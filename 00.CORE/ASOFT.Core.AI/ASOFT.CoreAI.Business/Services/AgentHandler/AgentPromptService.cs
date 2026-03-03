using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Business.LibraryKernel;
using ASOFT.CoreAI.Business.Services.PromptHandler;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.OpenApi.Exceptions;
using System.Text;
using static ASOFT.CoreAI.Business.SettingsManagerService;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class AgentPromptService
    {
        private IST2130Queries _agentPromptQueries;
        private readonly Kernel _kernel;
        private SettingsManagerService _settingsManagerService;
        private IChatResponseHandlerService _chatResponseHandlerService;

        public AgentPromptService(IST2130Queries agentPromptQueries,
            Kernel kernel, SettingsManagerService settingsManagerService,
            IChatResponseHandlerService chatResponseHandlerService)
        {
            _agentPromptQueries = agentPromptQueries;
            _kernel = kernel;
            _settingsManagerService = settingsManagerService;
            _chatResponseHandlerService = chatResponseHandlerService;
        }

        public async Task<string> GetPromptTemplate(string agentKey)
        {
            var prompt = await _agentPromptQueries.GetPromptByCode(agentKey);

            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return string.Empty;

            return prompt.PromptContent;
        }
        public async Task<ST2130> GetPromptByCode(string agentKey)
        {
            return await _agentPromptQueries.GetPromptByCode(agentKey);
        }

        public async Task<string> SendPromptWithReadFile<T>(
        ReadFileRequest request,
        string promptSystem,
        string promptTemplate,
        List<ResultReadFileModel>? awnserOCRs,
        IEnumerable<ChatHistoryResponseModel> chatHistory,
        IEnumerable<RedisearchResultItem> trainingData,
        List<T> datas,
        List<BEMF2001ViewModel> details, string? resultCreateFile = null)
        {
            try
            {
                string question = request.Question ?? string.Empty;
                var arguments = new KernelArguments
                {
                    ["UserId"] = request.UserId,
                    ["UserName"] = request.UserName,
                    ["CurrentTime"] = DateTime.Now,
                    ["question"] = question,
                    ["ocrFiles"] = awnserOCRs,
                    ["datas"] = datas,
                    ["details"] = !details.Any() ? null : details.Select(x => new
                    {
                        x.Description, // Mô tả
                        x.InvoiceNo, // Số hóa đơn
                        x.RequestAmount, // số tiền yêu cầu
                        x.InvoiceDate, // Ngày hóa đơn (định dạng)
                        x.RingiNo, // Số Ringi
                    }),
                    ["evaluationText"] = resultCreateFile,

                    ["chatHistory"] = chatHistory.Select(x => new
                    {
                        x.ResponseText,
                        x.Message,
                        x.CreateDate,
                        x.UserID
                    })
                };
                if (trainingData != null)
                {
                    arguments["trainingData"] = trainingData.Where(x => !string.IsNullOrEmpty(x.Text)).Select(x => new
                    {
                        x.Text,
                    });
                }
                return await HandleChatWithModelAI(promptSystem, arguments, request.IsStreaming, promptTemplate, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SendPromptWithLocalsAsync<T>(
        ReadFileRequest request,
        string promptSystem,
        string promptTemplate,
        string awnserOCRs,
        IEnumerable<ChatHistoryResponseModel> chatHistory,
        IEnumerable<RedisearchResultItem> trainingData,
        List<T> datas,
        List<BEMF2001ViewModel> details, string? resultCreateFile = null)
        {
            try
            {
                string question = request.Question ?? string.Empty;
                var arguments = new KernelArguments
                {
                    ["UserId"] = request.UserId,
                    ["UserName"] = request.UserName,
                    ["CurrentTime"] = DateTime.Now,
                    ["question"] = question,
                    ["content"] = awnserOCRs,
                    ["datas"] = datas,
                    ["details"] = !details.Any() ? null : details.Select(x => new
                    {
                        x.Description, // Mô tả
                        x.InvoiceNo, // Số hóa đơn
                        x.RequestAmount, // số tiền yêu cầu
                        x.InvoiceDate, // Ngày hóa đơn (định dạng)
                        x.RingiNo, // Số Ringi
                    }),
                    ["evaluationText"] = resultCreateFile,
                    ["chatHistory"] = chatHistory.Select(x => new
                    {
                        x.ResponseText,
                        x.Message,
                        x.CreateDate,
                        x.UserID
                    })
                };
                if (trainingData != null)
                {
                    arguments["trainingData"] = trainingData.Where(x => !string.IsNullOrEmpty(x.Text)).Select(x => new
                    {
                        x.Text,
                    });
                }
                return await HandleChatWithModelAI(promptSystem, arguments, request.IsStreaming, promptTemplate, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Xử lý gửi câu hỏi,lịch sử chat, thông tin training, thông tin dữ liệu từ Database sang ModelAI

        public async Task<string> SendPromptWithAgentAsync<T>(
            AgentRequest request,
            bool isCheckData,
            List<T> datas,
            IEnumerable<ChatHistoryResponseModel> chatHistory,
            string promptTemplate,
            IEnumerable<RedisearchResultItem> trainingData,
            CancellationToken cancellationToken = default)
        {
            if (isCheckData && (datas == null || datas.Count == 0))
                return "Mình chưa thấy dữ liệu nào từ bạn. Bạn có thể gửi lại thông tin không?";

            var arguments = new KernelArguments
            {
                ["UserId"] = request.UserId,
                ["UserName"] = request.UserName,
                ["CurrentTime"] = DateTime.Now,
                ["question"] = request.Question,
                ["datas"] = datas,
                ["trainingData"] = trainingData.Select(x => new
                {
                    x.Text,
                }),
                ["chatHistory"] = chatHistory.Select(x => new
                {
                    x.ResponseText,
                    x.Message,
                    x.CreateDate,
                    x.UserID
                })
            };
            return await HandleChatWithModelAI(request.Question, arguments, request.IsStreaming, promptTemplate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> SendPromptWithDocsAsync<T>(
            AgentRequest request,
            string promptTemplate,
            List<ResultReadFileModel>? awnserOCRs,
            IEnumerable<ChatHistoryResponseModel> chatHistory,
            IEnumerable<RedisearchResultItem> trainingData, List<T>? datas = null)
        {
            var arguments = new KernelArguments
            {
                ["UserId"] = request.UserId,
                ["UserName"] = request.UserName,
                ["CurrentTime"] = DateTime.Now,
                ["question"] = request.Question,
                ["ocrFiles"] = awnserOCRs,
                ["datas"] = datas,
                ["trainingData"] = trainingData.Select(x => new
                {
                    x.Text,
                }),
                ["chatHistory"] = chatHistory.Select(x => new
                {
                    x.ResponseText,
                    x.Message,
                    x.CreateDate,
                    x.UserID
                })
            };
            return await HandleChatWithModelAI(request.Question, arguments, request.IsStreaming, promptTemplate, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task<string> HandleChatWithModelAI(string promptSystem, KernelArguments arguments, bool isStreaming, string promptTemplate, CancellationToken cancellationToken)
        {
            var configLLM = await _settingsManagerService.GetConfigLLMsAsync();
            string resultResponse = string.Empty;
            if (isStreaming)
            {
                if (configLLM.IsUse)
                {
                    var response = await _chatResponseHandlerService.InvokePromptAsync(promptSystem, promptTemplate, arguments);
                    resultResponse = response.Text ?? string.Empty;
                }
                else
                {
                    var resultStream = _kernel.InvokePromptStreamingAsync(
                   promptTemplate,
                   arguments,
                   "handlebars",
                   new HandlebarsPromptTemplateFactory(),
                   cancellationToken);

                    var sb = new StringBuilder();
                    await foreach (var msg in resultStream)
                        sb.Append(msg);
                    resultResponse = sb.ToString();
                }
            }
            else
            {
                try
                {
                    if (configLLM.IsUse)
                    {
                        var response = await _chatResponseHandlerService.InvokePromptAsync(promptSystem, promptTemplate, arguments);
                        resultResponse = response.Text ?? string.Empty;
                    }
                    else
                    {
                        var result = await _kernel.InvokePromptAsync(
                        promptTemplate,
                        arguments,
                        "handlebars",
                        new HandlebarsPromptTemplateFactory(),
                        cancellationToken);
                        resultResponse = result.ToString();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return resultResponse;
        }

        public async Task<string> SendPromptWithSumaryResultAsync(string promptSystem, string promptTemplate, string result)
        {
            var arguments = new KernelArguments
            {
                ["result"] = result,
            };
            //string titleDefault = "Bạn là trợ lý AI tóm tắt và tổng hợp kết quả.";
            return await HandleChatWithModelAI(promptSystem, arguments, false, promptTemplate, CancellationToken.None).ConfigureAwait(false);
        }

        #endregion Xử lý gửi câu hỏi,lịch sử chat, thông tin training, thông tin dữ liệu từ Database sang ModelAI

        // Hàm BuildQueryFromRawText cũng generic, dùng cho bất kỳ loại nào
        public async Task<List<T>> BuildQueryFromRawText<T>(
            AgentRequest request,
            IEnumerable<RedisearchResultItem> redisearchResultItems,
            IEnumerable<ChatHistoryResponseModel> chatHistory,
            List<T> itemList,
            string promptTemplate)
        {
            var resultItems = new List<T>();

            var result = await SendPromptWithAgentAsync(request, false, resultItems, chatHistory, promptTemplate, redisearchResultItems);

            // Giả sử result.ToString() trả về JSON chứa filters
            var filters = BuildQueryPrompt.ExtractConditionsFromResponse(result.ToString());

            if (filters == null || filters.Count == 0)
            {
                return itemList;
            }
            resultItems = BuildQueryPrompt.QueryByFilters(itemList, filters);
            return resultItems;
        }
        public async Task<AIKeyStatus> CheckKeyWithAsync()
        {
            try
            {
                var result = await _kernel.InvokePromptAsync("ping");
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

    }
}