using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatResponseRepository : IRepository<ST2151>
    {
        Task<IEnumerable<ST2151>> GetByChatMessageIdAsync(Guid chatMessageId);
    }
}