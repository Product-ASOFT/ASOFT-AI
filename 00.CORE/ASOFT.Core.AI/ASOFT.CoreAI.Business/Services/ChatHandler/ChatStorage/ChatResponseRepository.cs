using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business.Services.ChatHandler.ChatStorage
{
    public class ChatResponseRepository : IChatResponseRepository
    {
        private readonly IBusinessContext<ST2134> _chatResponseContext;

        public ChatResponseRepository(IBusinessContext<ST2134> chatResponseContext)
        {
            _chatResponseContext = Checker.NotNull(chatResponseContext, nameof(chatResponseContext));
        }

        public async Task<bool> AddAsync(ST2134 chatResponse, CancellationToken cancellationToken = default)
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

        public async Task<bool> UpdateAsync(ST2134 entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ST2134>> IRepository<ST2134>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ST2134>> IChatResponseRepository.GetByChatMessageIdAsync(Guid chatMessageId)
        {
            throw new NotImplementedException();
        }

        Task<ST2134> IRepository<ST2134>.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}