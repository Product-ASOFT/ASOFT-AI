using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatResponseRepository : IRepository<ST2134>
    {
        Task<IEnumerable<ST2134>> GetByChatMessageIdAsync(Guid chatMessageId);
    }
}