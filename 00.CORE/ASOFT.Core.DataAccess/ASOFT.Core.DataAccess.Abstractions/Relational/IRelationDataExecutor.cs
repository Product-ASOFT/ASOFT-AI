using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Relational.Transactions;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ASOFT.Core.DataAccess.Relational
{
    /// <summary>
    /// Executor
    /// </summary>
    public interface IRelationDataExecutor
    {
        /// <summary>
        /// Set <see cref="IDbTransaction"/>.
        /// </summary>
        /// <param name="dbTransaction"></param>
        void SetTransaction(IDbTransaction dbTransaction);

        /// <summary>
        /// Set <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="dbConnection"></param>
        void SetConnection(IDbConnection dbConnection);

        /// <summary>
        /// Set <see cref="IDbConnectionProvider"/>.
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        void SetDbConnectionProvider(IDbConnectionProvider dbConnectionProvider);

        /// <summary>
        /// Set <see cref="TransactionScope"/>.
        /// </summary>
        /// <param name="transactionScope"></param>
        void SetTransactionScope(TransactionScope transactionScope);

        /// <summary>
        /// See <see cref="TransactionHolder"/>.
        /// </summary>
        /// <param name="transactionHolder"></param>
        void SetTransactionHolder(TransactionHolder transactionHolder);

        /// <summary>
        /// Run query and return data with connection.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UseConnectionAsync(Func<IDbConnection, Task> taskCreator,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Run query and return data with connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> UseConnectionAsync<T>(Func<IDbConnection, Task<T>> taskCreator,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sử dụng transaction.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UseTransactionAsync(Func<IDbConnection, TransactionHolder, Task> taskCreator,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sử dụng transaction.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> UseTransactionAsync<T>(Func<IDbConnection, TransactionHolder, Task<T>> taskCreator,
            CancellationToken cancellationToken = default);
    }
}