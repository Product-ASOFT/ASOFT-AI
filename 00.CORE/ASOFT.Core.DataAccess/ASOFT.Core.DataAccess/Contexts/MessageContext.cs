using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess.Cache;
using ASOFT.Core.DataAccess.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess
{
    public class MessageContext : EFGenericContext<Message, BusinessDbContext>, IMessageContext
    {
        private readonly IMemoryCache _memoryCache;

        public MessageContext(BusinessDbContext context, IMemoryCache memoryCache) :
            base(context)
        {
            _memoryCache = Checker.NotNull(memoryCache, nameof(memoryCache));
        }

        public Message GetByID(string id, string languageID)
        {
            return EntitySet(false).Where(m => id == m.ID && m.LanguageID == languageID).FirstOrDefault();
        }

        public async Task<Message> GetByIDAsync(string id, string languageID,
            CancellationToken cancellationToken = default)
        {
            var result = await EntitySet(false).Where(m => id == m.ID && m.LanguageID == languageID).FirstOrDefaultAsync();

            if(result == null) result = await EntitySet(false).Where(m => id == m.ID).FirstOrDefaultAsync();

            return result;
        }

        public virtual IEnumerable<Message> GetByIDs(string languageID, params string[] ids)
           => EntitySet(false).Where(m => ids.Contains(m.ID) && m.LanguageID == languageID);

        public virtual IEnumerable<Message> GetByModule(string module, string languageID) =>
            EntitySet(false).Where(m => m.Module == module && m.LanguageID == languageID);

        public virtual async Task<List<Message>> GetByModuleAsync(string module, string languageID) =>
            await EntitySet(false).Where(m => m.Module == module && m.LanguageID == languageID).ToListAsync();
    }
}