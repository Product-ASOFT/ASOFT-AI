using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatSessionRepository : IRepository<ST2131>
    {
        Task<ST2131> GetByUserIdAsync(Guid ID, string userId);
    }
}