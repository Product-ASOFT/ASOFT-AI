using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class ChatResponseRepository : IChatResponseRepository
    {
        private readonly IBusinessContext<ST2151> _chatResponseContext;

        public ChatResponseRepository(IBusinessContext<ST2151> chatResponseContext)
        {
            _chatResponseContext = Checker.NotNull(chatResponseContext, nameof(chatResponseContext));
        }

        public async Task<bool> AddAsync(ST2151 chatResponse, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _chatResponseContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _chatResponseContext.AddAsync(chatResponse, cancellationToken);
                    await _chatResponseContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(ST2151 entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ST2151>> IRepository<ST2151>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ST2151>> IChatResponseRepository.GetByChatMessageIdAsync(Guid chatMessageId)
        {
            throw new NotImplementedException();
        }

        Task<ST2151> IRepository<ST2151>.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}