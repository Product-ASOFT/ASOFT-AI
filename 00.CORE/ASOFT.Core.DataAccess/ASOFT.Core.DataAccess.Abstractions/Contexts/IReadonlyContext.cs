using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.UnitOfWorks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Generic readonly cho repository pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TUnitOfWork"></typeparam>
    public interface IReadonlyContext<T, out TUnitOfWork> : IRootContext<TUnitOfWork> where T : class
        where TUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Truy vấn entity.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        IQueryable<T> Query(ISourceQuery<T> spec);

        /// <summary>
        /// Truy vấn entity.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync(ISourceQuery<T> spec);

        /// <summary>
        /// Truy vấn entity đầu tiên.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync(ISourceQuery<T> spec);
    }
}