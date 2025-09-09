using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatFileRepository : IRepository<ST2161>
    {
        Task<IEnumerable<ST2161>> GetByChatMessageIdAsync(Guid chatMessageId);
    }
}