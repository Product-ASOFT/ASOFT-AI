using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatMessageRepository : IRepository<ST2133>
    {
        Task<IEnumerable<ST2133>> GetBySessionIdAsync(Guid sessionId);
    }
}