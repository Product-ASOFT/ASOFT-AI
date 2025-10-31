using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatHistoryHandler
    {
        Task<ST2133> SaveChatMessageAsync(ChatHistoryModel chatHistory, CancellationToken cancellationToken = default);

        Task<bool> SaveChatResponseAsync(string answer, ST2133 chatMessages);

        Task<IEnumerable<ChatHistoryResponseModel>> GetChatHistoryAsync(ChatHistoryModel chatHistory, CancellationToken cancellationToken = default);
    }
}