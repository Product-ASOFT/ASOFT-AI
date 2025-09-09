using ASOFT.Core.DataAccess.Entites;
using ASOFT.Core.DataAccess.Relational.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess
{
    public interface ILanguageContext : IBulkRepository<Language>
    {
        /// <summary>
        /// Get language by form
        /// </summary>
        /// <param name="formID"></param>
        /// <param name="module"></param>
        /// <param name="languageID"></param>
        /// <returns></returns>
        IEnumerable<Language> GetByForm(string formID, string module, string languageID);

        /// <summary>
        /// Get language by form async
        /// </summary>
        /// <param name="formID"></param>
        /// <param name="module"></param>
        /// <param name="languageID"></param>
        /// <returns></returns>
        Task<IEnumerable<Language>> GetByFormAsync(string formID, string module, string languageID);
    }
}