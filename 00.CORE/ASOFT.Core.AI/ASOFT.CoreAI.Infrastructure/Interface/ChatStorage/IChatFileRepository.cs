using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatFileRepository : IRepository<ST2135>
    {
        Task<IEnumerable<ST2135>> GetByChatMessageIdAsync(Guid chatMessageId);
    }
}