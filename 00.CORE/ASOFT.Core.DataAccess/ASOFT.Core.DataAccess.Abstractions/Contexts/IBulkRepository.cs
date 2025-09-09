using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess.Relational.Context
{
    /// <summary>
    /// Bulk repository hỗ trợ bulk insert, update, delete.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBulkRepository<T> : IRelationalContext<T> where T : class
    {
        /// <summary>
        /// Thêm mới số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        Task BulkAddAsync(IEnumerable<T> entities, BulkOptions bulkOptions = default);

        /// <summary>
        /// Thêm mới hoặc cập nhật số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        Task BulkAddOrUpdateAsync(IEnumerable<T> entities, BulkOptions bulkOptions = default);

        /// <summary>
        /// Cập nhật số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        Task BulkUpdateAsync(IEnumerable<T> entities, BulkOptions bulkOptions = default);

        /// <summary>
        /// Xóa số lượng lớn entity.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="bulkOptions"></param>
        /// <returns></returns>
        Task BulkDeleteAsync(IEnumerable<T> entities, BulkOptions bulkOptions = default);
    }
}