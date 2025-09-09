using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatMessageRepository : IRepository<ST2141>
    {
        Task<IEnumerable<ST2141>> GetBySessionIdAsync(Guid sessionId);
    }
}