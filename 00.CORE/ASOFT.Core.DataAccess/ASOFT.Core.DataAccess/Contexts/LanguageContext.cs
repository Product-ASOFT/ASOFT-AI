using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Entites;

namespace ASOFT.Core.DataAccess
{
    public class LanguageContext : EFGenericContext<Language, BusinessDbContext>, ILanguageContext
    {
        public LanguageContext(BusinessDbContext context) : base(context)
        {
        }

        public virtual IEnumerable<Language> GetByForm(string formID, string module, string languageID)
            => EntitySet().AsNoTracking()
                .Where(m => m.Module == module && m.FormID == formID && m.LanguageID == languageID);

        public virtual async Task<IEnumerable<Language>> GetByFormAsync(string formID, string module, string languageID)
            => await EntitySet().AsNoTracking()
                .Where(m => m.Module == module && m.FormID == formID && m.LanguageID == languageID).ToListAsync();
    }
}