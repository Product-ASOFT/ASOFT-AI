using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business.Services.ChatHandler.ChatStorage
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly IBusinessContext<ST2133> _chatMessageContext;

        public ChatMessageRepository(IBusinessContext<ST2133> chatMessageContext)
        {
            _chatMessageContext = Checker.NotNull(chatMessageContext, nameof(chatMessageContext));
        }

        public async Task<bool> AddAsync(ST2133 chatMessage, CancellationToken cancellationToken = default)

        {
            try
            {
                return await _chatMessageContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _chatMessageContext.AddAsync(chatMessage, cancellationToken);
                    await _chatMessageContext.UnitOfWork.CompleteAsync();
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

        public Task<IEnumerable<ST2133>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ST2133> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ST2133>> GetBySessionIdAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(ST2133 entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}