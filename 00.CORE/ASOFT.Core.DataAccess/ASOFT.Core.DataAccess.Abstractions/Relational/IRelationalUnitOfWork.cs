using ASOFT.Core.DataAccess.Relational.Transactions;
using ASOFT.Core.DataAccess.UnitOfWorks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess.Relational
{
    /// <summary>
    /// Unit of work cho relational database
    /// </summary>
    public interface IRelationalUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Allowed execute insert, update, delete in transaction async.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteInTransactionAsync(Func<TransactionHolder, Task> taskCreator,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Allowed execute insert, update, delete in transaction async.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> ExecuteInTransactionAsync<T>(Func<TransactionHolder, Task<T>> taskCreator,
            CancellationToken cancellationToken = default);
    }
}