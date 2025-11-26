using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Dapper;
using System.Data;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business.Services.ChatHandler
{
    public class ChatHistoryHandler : BusinessDataAccess, IChatHistoryHandler
    {
        private readonly IChatFileRepository _chatFileRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatResponseRepository _chatResponseRepository;
        private readonly IChatSessionRepository _chatSessionRepository;

        public ChatHistoryHandler(
            IDbConnectionProvider dbConnectionProvider,
            IChatFileRepository chatFileRepository,
            IChatMessageRepository chatMessageRepository,
            IChatResponseRepository chatResponseRepository,
            IChatSessionRepository chatSessionRepository) : base(dbConnectionProvider)
        {
            _chatFileRepository = chatFileRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatResponseRepository = chatResponseRepository;
            _chatSessionRepository = chatSessionRepository;
        }

        #region Xử lý lưu trữ lịch sử chat

        public async Task<ST2133> SaveChatMessageAsync(ChatHistoryModel chatHistory, CancellationToken cancellationToken = default)
        {
            var chatSession = await CreateChatSessionAsync(chatHistory);
            if (chatSession == null)
                return new ST2133();

            return await CreateChatMessageAsync(chatHistory, chatSession.APK);
        }

        public async Task<bool> SaveChatResponseAsync(string answer, ST2133 chatMessage)
        {
            if (string.IsNullOrEmpty(answer))
                throw new ArgumentNullException(nameof(answer));
            if (chatMessage == null)
                throw new ArgumentNullException(nameof(chatMessage));

            await CreateChatResponseAsync(chatMessage.CreateUserID, answer, chatMessage.APK);
            return true;
        }

        private async Task<ST2132?> CreateChatSessionAsync(ChatHistoryModel chatHistory)
        {
            ST2132? chatSession = null;
            if (chatHistory.ChatSessionID != Guid.Empty && chatHistory.ChatSessionID.HasValue)
            {
                chatSession = await _chatSessionRepository.GetByUserIdAsync(chatHistory.ChatSessionID.Value, chatHistory.UserID);
            }
            if (chatSession == null)
            {
                chatSession = new ST2132
                {
                    APK = Guid.NewGuid(),
                    CreateUserID = chatHistory.UserID,
                    Status = ChatSessionStatus.Active.ToString(),
                    CreateDate = DateTime.Now,
                };

                var added = await _chatSessionRepository.AddAsync(chatSession);
                if (!added)
                    return null;
            }
            else
            {
                chatSession.LastModifyDate = DateTime.Now;
                chatSession.LastModifyUserID = chatHistory.UserID;

                await _chatSessionRepository.UpdateAsync(chatSession);
            }

            return chatSession;
        }

        private async Task<ST2133> CreateChatMessageAsync(ChatHistoryModel chatHistory, Guid chatSessionId)
        {
            var chatMessage = new ST2133
            {
                APK = Guid.NewGuid(),
                ChatSessionID = chatSessionId,
                CreateUserID = chatHistory.UserID,
                MessageTime = DateTime.Now,
                Message = chatHistory.Question,
                IsUserMessage = true,
                TypeChat = chatHistory.TypeChat,
                ModuleName = chatHistory.ModuleName,
                AgentCode = chatHistory.AgentCode,
                CreateDate = DateTime.Now,
            };

            await _chatMessageRepository.AddAsync(chatMessage);
            return chatMessage;
        }

        private async Task<ST2134> CreateChatResponseAsync(string userId, string answer, Guid chatMessageId)
        {
            var chatResponse = new ST2134
            {
                APK = Guid.NewGuid(),
                ChatMessageID = chatMessageId,
                ResponseText = answer,
                ResponseTime = DateTime.Now,
                ResponseType = ChatResponseType.AI.ToString(),
                CreateUserID = userId,
                CreateDate = DateTime.Now,
            };

            await _chatResponseRepository.AddAsync(chatResponse);
            return chatResponse;
        }

        public async Task<ST2135> CreateChatFileAsync(ChatHistoryModel chatHistory, Guid chatMessageId)
        {
            var chatFile = new ST2135
            {
                APK = Guid.NewGuid(),
                CreateUserID = chatHistory.UserID,
                ChatMessageID = chatMessageId,
                FileName = chatHistory.FileName ?? string.Empty,
                FileType = chatHistory?.FileType,
                FileUrl = chatHistory?.FileUrl,
                FileData = chatHistory.FileData,
                UploadedAt = DateTime.Now,
                CreateDate = DateTime.Now,
            };

            await _chatFileRepository.AddAsync(chatFile);
            return chatFile;
        }

        #endregion Xử lý lưu trữ lịch sử chat

        #region Lấy thông tin lịch sử chat

        public async Task<IEnumerable<ChatHistoryResponseModel>> GetChatHistoryAsync(ChatHistoryModel chatHistory, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ChatSessionID", chatHistory.ChatSessionID, DbType.Guid, ParameterDirection.Input);
            parameters.Add("@UserID", chatHistory.UserID, DbType.String, ParameterDirection.Input);
            parameters.Add("@AgentCode", chatHistory.AgentCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@ModuleName", chatHistory.ModuleName, DbType.String, ParameterDirection.Input);
            parameters.Add("@TypeChat", chatHistory.TypeChat, DbType.String, ParameterDirection.Input);
            parameters.Add("@PageNumber", chatHistory.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", chatHistory.PageSize, DbType.Int32, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection
                => await connection.QueryAsync<ChatHistoryResponseModel>
                ("SP2130", parameters, commandType: CommandType.StoredProcedure), cancellationToken);
        }

        #endregion Lấy thông tin lịch sử chat
        public ChatHistoryModel CreateChatHistoryModel(AgentRequest request, string agentCode, int maxChat)
        {
            return new ChatHistoryModel
            {
                ChatSessionID = request.ChatSessionID,
                UserID = request.UserId,
                Question = request.Question,
                TypeChat = EnumConstants.TypeChat.Plugin.ToString(),
                ModuleName = request.Module,
                AgentCode = agentCode,
                PageNumber = 1,
                PageSize = maxChat,
            };
        }
    }
}