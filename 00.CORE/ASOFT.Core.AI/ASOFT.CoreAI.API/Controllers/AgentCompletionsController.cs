using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.API.Resources;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Business.LibraryKernel;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.OO.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text;
using static ASOFT.CoreAI.Common.AIConstants;

namespace ASOFT.CoreAI.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class AgentCompletionsController : AgentBaseController
    {
        private readonly ChatCompletionAgent _agent;
        private readonly ChatHistory _chatHistory;
        private readonly Kernel _kernel;
        private IST2130Queries _agentPromptQueries;
        private IChatHistoryHandler _chatHistoryHandler;
        private IChatResponseHandlerService _chatResponseHandlerService;
        private SettingsManagerService _settingsManagerService;

        public AgentCompletionsController(ChatCompletionAgent agent, Kernel kernel,
            IST2130Queries agentPromptQueries, SettingsManagerService settingsManagerService,
            IChatHistoryHandler chatHistoryHandler, AgentManagerService agentManager, IChatResponseHandlerService chatResponseHandlerService)
        {
            this._agent = agent;
            this._chatHistory = [];
            this._kernel = kernel;
            this._agentPromptQueries = agentPromptQueries;
            _chatHistoryHandler = chatHistoryHandler;
            _chatResponseHandlerService = chatResponseHandlerService;
            _settingsManagerService = settingsManagerService;
        }

        // Gọi API xử lý chat
        [ActionName("ChatComplete")]
        [HttpPost]
        public async Task<ChatResponseModel> CompleteAsync([FromBody] AgentRequest request, CancellationToken cancellationToken)
        {
            var requestCompletion = new AgentCompletionRequest
            {
                Prompt = request.Question,
                ChatHistory = this._chatHistory,
            };
            ValidateChatHistory(requestCompletion.ChatHistory);

            // Add the "question" argument used in the agent template.
            var arguments = new KernelArguments
            {
                ["question"] = request.Question
            };
            #region tạo thông tin request cho lịch sử chat

            string typeChat = EnumConstants.TypeChat.Normal.ToString();
            var chatHistoryModel = new ChatHistoryModel
            {
                ChatSessionID = request.ChatSessionID,
                UserID = request.UserId,
                Question = request.Question,
                TypeChat = typeChat,
                PageNumber = 1,
                PageSize = 10,
            };

            #endregion tạo thông tin request cho lịch sử chat

            #region lấy lịch sử chat từ cơ sở dữ liệu

            IEnumerable<ChatHistoryResponseModel> chatHistory = await _chatHistoryHandler.GetChatHistoryAsync(chatHistoryModel);
            foreach (var item in chatHistory)
            {
                requestCompletion.ChatHistory.AddUserMessage(item.ResponseText);
            }

            #endregion lấy lịch sử chat từ cơ sở dữ liệu

            #region lưu lịch sử của câu hỏi vào Database

            var chatMessages = await _chatHistoryHandler.SaveChatMessageAsync(chatHistoryModel);

            #endregion lưu lịch sử của câu hỏi vào Database

            requestCompletion.ChatHistory.AddUserMessage(request.Question);
            string responseMessage = string.Empty;

            var configLLM = await _settingsManagerService.GetConfigLLMsAsync();
            if (configLLM != null && configLLM.IsUse)
            {
                string promptSystem = "Bạn là trợ lý ảo thông minh, giúp hỗ trợ trả lời các câu hỏi của người dùng một cách chính xác và nhanh chóng.";
                var resultResponse = await _chatResponseHandlerService.InvokeAsync(promptSystem, request.Question);
                responseMessage = resultResponse.Text ?? string.Empty;
            }
            else
            {
                var sb = new StringBuilder();
                if (request.IsStreaming)
                {
                    var streamingMessages = this.CompleteSteamingAsync(requestCompletion.ChatHistory, arguments, cancellationToken);

                    await foreach (var messageContent in streamingMessages)
                    {
                        sb.Append(messageContent.Content);
                    }
                    responseMessage = sb.ToString();
                }
                else
                {
                    var chatMessageContents = this.CompleteAsync(requestCompletion.ChatHistory, arguments, cancellationToken);
                    await foreach (var messageContent in chatMessageContents)
                    {
                        sb.Append(messageContent.Content);
                    }
                    responseMessage = sb.ToString();
                }
            }
            #region Lưu câu trả lời vào Database

            Guid chatSessionID = Guid.Empty;
            if (chatMessages != null && chatMessages.APK != chatSessionID)
            {
                chatSessionID = chatMessages.ChatSessionID;
                await _chatHistoryHandler.SaveChatResponseAsync(responseMessage, chatMessages);
            }

            #endregion Lưu câu trả lời vào Database

            return ChatHandlerHelper.CreateResponse(chatSessionID, responseMessage, true);
        }

        private async IAsyncEnumerable<ChatMessageContent> CompleteAsync(ChatHistory chatHistory, KernelArguments arguments, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var thread = new ChatHistoryAgentThread(chatHistory);
            IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> content =
                this._agent.InvokeAsync(thread, options: new() { KernelArguments = arguments }, cancellationToken: cancellationToken);

            await foreach (ChatMessageContent item in content.ConfigureAwait(false))
            {
                yield return item;
            }
        }

        // Gọi hàm xử lý chat streaming
        private async IAsyncEnumerable<StreamingChatMessageContent> CompleteSteamingAsync(ChatHistory chatHistory, KernelArguments arguments, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var thread = new ChatHistoryAgentThread(chatHistory);
            IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> content =
                this._agent.InvokeStreamingAsync(thread, options: new() { KernelArguments = arguments }, cancellationToken: cancellationToken);

            await foreach (StreamingChatMessageContent item in content.ConfigureAwait(false))
            {
                yield return item;
            }
        }

        // validate dữ liệu chat theo vai trò sử dụng
        private static void ValidateChatHistory(ChatHistory chatHistory)
        {
            foreach (ChatMessageContent content in chatHistory)
            {
                if (content.Role == AuthorRole.System)
                {
                    throw new ArgumentException("A system message is provided by the agent and should not be included in the chat history.");
                }
            }
        }

        // Gọi hàm phân loại câu hỏi Chat thường hay AI Plugin
        [HttpPost]
        [ActionName("ClassifyQuestionType")]
        public async Task<ChatResponseModel> ClassifyQuestionTypeAsync([FromBody] AgentRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return ChatHandlerHelper.CreateResponse(request.ChatSessionID, "Câu hỏi không được để trống!", false);
            }
            return await GetTypeQuestionAsync(request, cancellationToken);
        }

        // Hàm phân loại câu hỏi để xác định là câu hỏi thường hay câu hỏi AI Plugin
        private async Task<ChatResponseModel> GetTypeQuestionAsync(AgentRequest agentRequest, CancellationToken cancellationToken)
        {
            var prompt = await _agentPromptQueries.GetPromptByCode(AgentKeys.TYPE_QUESTION);
            if (prompt == null || string.IsNullOrEmpty(prompt.PromptContent))
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Hiện tại bạn chưa tạo Prompt để phân loại câu hỏi. Vui lòng thiết lập Prompt để tiếp tục.", false);
            }
            var chatHistoryModel = _chatHistoryHandler.CreateChatHistoryModel(agentRequest, string.Empty, 10);
            var chatHistory = await _chatHistoryHandler.GetChatHistoryAsync(chatHistoryModel);
            var arguments = new KernelArguments
            {
                ["question"] = agentRequest.Question,
                ["chatHistory"] = chatHistory.Select(x => new
                {
                    x.ResponseText,
                    x.Message,
                    x.CreateDate,
                    x.UserID
                })
            };
            string responseMessage = string.Empty;
            try
            {
                var configLLM = await _settingsManagerService.GetConfigLLMsAsync();
                if (configLLM != null && configLLM.IsUse)
                {
                    string promptSystem = " Bạn là trợ lý ảo thông minh, giúp hỗ trợ trả lời các câu hỏi của người dùng một cách chính xác và nhanh chóng.";
                    var result = await _chatResponseHandlerService.InvokePromptAsync(promptSystem, prompt.PromptContent, arguments);
                    responseMessage = result.Text ?? string.Empty;
                }
                else
                {
                    var result = await _kernel.InvokePromptAsync(
                         promptTemplate: prompt.PromptContent,
                         arguments: arguments,
                         templateFormat: "handlebars",
                         promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
                         cancellationToken: cancellationToken
                    );
                    responseMessage = result.ToString();
                }
            }
            catch (Exception)
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Kết nối đến AI bị gián đoạn", false);
            }
            return ChatHandlerHelper.CreateResponse(agentRequest.ChatSessionID, responseMessage, true);
        }
    }
}