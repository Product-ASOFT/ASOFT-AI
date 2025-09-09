using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class ChatFileRepository : IChatFileRepository
    {
        private readonly IBusinessContext<ST2161> _chatFilesContext;

        public ChatFileRepository(IBusinessContext<ST2161> chatFilesContext)
        {
            _chatFilesContext = Checker.NotNull(chatFilesContext, nameof(chatFilesContext));
        }

        public Task<bool> AddAsync(ST2161 entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ST2161>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ST2161>> GetByChatMessageIdAsync(Guid chatMessageId)
        {
            throw new NotImplementedException();
        }

        public Task<ST2161> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(ST2161 entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}