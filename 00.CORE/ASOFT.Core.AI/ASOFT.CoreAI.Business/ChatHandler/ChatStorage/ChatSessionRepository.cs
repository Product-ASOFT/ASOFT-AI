using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly IBusinessContext<ST2131> _chatSessionContext;

        public ChatSessionRepository(IBusinessContext<ST2131> chatSessionContext)
        {
            _chatSessionContext = Checker.NotNull(chatSessionContext, nameof(chatSessionContext));
        }

        public async Task<bool> AddAsync(ST2131 chatSession, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _chatSessionContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _chatSessionContext.AddAsync(chatSession, cancellationToken);
                    await _chatSessionContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ST2131> GetByUserIdAsync(Guid ID, string userId)
        {
            var result = await _chatSessionContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2131>(m => m.APK == ID && m.CreateUserID == userId));
            return result;
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ST2131>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ST2131> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(ST2131 chatSession, CancellationToken cancellationToken)
        {
            try
            {
                return await _chatSessionContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _chatSessionContext.UpdateAsync(chatSession, cancellationToken);
                    await _chatSessionContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}