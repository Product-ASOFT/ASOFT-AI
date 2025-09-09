using ASOFT.Core.DataAccess.Entites;
using ASOFT.Core.DataAccess.Relational.Context;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess
{
    public interface IMessageContext : IBulkRepository<Message>
    {
        /// <summary>
        /// Lấy message theo id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageID"></param>
        /// <returns></returns>
        Message GetByID(string id, string languageID);

        /// <summary>
        /// Lấy danh sách message theo nhiều id truyền vào.
        /// </summary>
        /// <param name="languageID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<Message> GetByIDs(string languageID, params string[] ids);

        /// <summary>
        /// Lấy message theo id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Message> GetByIDAsync(string id, string languageID,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lấy message theo module.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="languageID"></param>
        /// <returns></returns>
        IEnumerable<Message> GetByModule(string module, string languageID);

        /// <summary>
        /// Lấy message theo module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="languageID"></param>
        /// <returns></returns>
        Task<List<Message>> GetByModuleAsync(string module, string languageID);
    }
}