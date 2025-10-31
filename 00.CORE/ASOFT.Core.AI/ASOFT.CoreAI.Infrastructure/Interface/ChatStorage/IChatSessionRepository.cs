using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatSessionRepository : IRepository<ST2132>
    {
        Task<ST2132> GetByUserIdAsync(Guid ID, string userId);
    }
}