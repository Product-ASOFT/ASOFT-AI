using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.API.Resources;
using ASOFT.CoreAI.Business;
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
        private IST2111Queries _agentPromptQueries;
        private IChatHistoryHandler _chatHistoryHandler;
        private AgentManager _agentManager;

        public AgentCompletionsController(ChatCompletionAgent agent, Kernel kernel,
            IST2111Queries agentPromptQueries,
            IChatHistoryHandler chatHistoryHandler, AgentManager agentManager)
        {
            this._agent = agent;
            this._chatHistory = [];
            this._kernel = kernel;
            this._agentPromptQueries = agentPromptQueries;
            _chatHistoryHandler = chatHistoryHandler;
            _agentManager = agentManager;
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

            #region Lưu câu trả lời vào Database

            Guid chatSessionID = Guid.Empty;
            if (chatMessages != null)
            {
                chatSessionID = chatMessages.ChatSessionID;
                await _chatHistoryHandler.SaveChatResponseAsync(responseMessage, chatMessages);
            }

            #endregion Lưu câu trả lời vào Database

            return ChatHandlerHelper.CreateResponse(chatSessionID, responseMessage);
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
                return ChatHandlerHelper.CreateResponse(request.ChatSessionID, "Câu hỏi không được để trống!");
            }
            var typeQuestion = await GetTypeQuestionAsync(request, cancellationToken);
            return typeQuestion;
        }

        // Hàm phân loại câu hỏi để xác định là câu hỏi thường hay câu hỏi AI Plugin
        private async Task<ChatResponseModel> GetTypeQuestionAsync(AgentRequest agentRequest, CancellationToken cancellationToken)
        {
            var prompt = await _agentPromptQueries.QueryPromptsByAgentCode(AgentKeys.TYPE_QUESTION);
            if (prompt == null || string.IsNullOrEmpty(prompt.PromptContent))
            {
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "Hiện tại bạn chưa tạo Prompt để phân loại câu hỏi. Vui lòng thiết lập Prompt để tiếp tục.");
            }
            var chatHistoryModel = _agentManager.CreateChatHistoryModel(agentRequest, string.Empty, 10);
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
                var result = await _kernel.InvokePromptAsync(
                     promptTemplate: prompt.PromptContent,
                     arguments: arguments,
                     templateFormat: "handlebars",
                     promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
                     cancellationToken: cancellationToken
                );
                responseMessage = result.ToString();
            }
            catch (Exception)
            {
                throw;
            }
            return ChatHandlerHelper.CreateResponse(Guid.Empty, responseMessage);
        }
    }
}