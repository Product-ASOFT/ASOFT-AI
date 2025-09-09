using ASOFT.Core.DataAccess.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable PossibleNullReferenceException

namespace ASOFT.Core.DataAccess.Relational.Transactions
{
    /// <summary>
    /// Util class for create transaction with unit of work pattern.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class RelationalUnitOfWorkTransaction
    {
        private readonly HashSet<IRelationalUnitOfWork> _unitOfWorks;

        /// <summary>
        /// Transaction with unit of work.
        /// </summary>
        /// <param name="unitOfWorks"></param>
        private RelationalUnitOfWorkTransaction(params IRelationalUnitOfWork[] unitOfWorks)
            => _unitOfWorks =
                new HashSet<IRelationalUnitOfWork>(unitOfWorks ?? throw new ArgumentNullException(nameof(unitOfWorks)),
                    UnitOfWorkComparer.Instance);

        private RelationalUnitOfWorkTransaction(IEnumerable<IRelationalUnitOfWork> unitOfWorks)
            => _unitOfWorks =
                new HashSet<IRelationalUnitOfWork>(unitOfWorks ?? throw new ArgumentNullException(nameof(unitOfWorks)),
                    UnitOfWorkComparer.Instance);

        /// <summary>
        /// Execute with transaction.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <returns></returns>
        public virtual async Task ExecuteInTransaction(Func<TransactionHolder, Task> taskCreator)
        {
            if (taskCreator == null)
            {
                throw new ArgumentNullException(nameof(taskCreator));
            }

            if (_unitOfWorks.Count == 1)
            {
                await _unitOfWorks.FirstOrDefault().ExecuteInTransactionAsync(taskCreator).ConfigureAwait(false);
            }
            else
            {
                using (var transactionScope = TransactionScopeHelper.CreateScopeAsyncFlow())
                {
                    await taskCreator(new TransactionHolder(transactionScope)).ConfigureAwait(false);
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Execute with transaction return data.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<T> ExecuteInTransaction<T>(Func<TransactionHolder, Task<T>> taskCreator)
        {
            if (taskCreator == null)
            {
                throw new ArgumentNullException(nameof(taskCreator));
            }

            if (_unitOfWorks.Count == 1)
            {
                return await _unitOfWorks.FirstOrDefault().ExecuteInTransactionAsync(taskCreator).ConfigureAwait(false);
            }

            using (var transactionScope = TransactionScopeHelper.CreateScopeAsyncFlow())
            {
                var result = await taskCreator(new TransactionHolder(transactionScope)).ConfigureAwait(false);
                transactionScope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Create new unit of work transaction
        /// </summary>
        /// <param name="unitOfWorks"></param>
        /// <returns></returns>
        public static RelationalUnitOfWorkTransaction New(IEnumerable<IRelationalUnitOfWork> unitOfWorks)
            => new RelationalUnitOfWorkTransaction(unitOfWorks);

        /// <summary>
        /// Create new unit of work transaction
        /// </summary>
        /// <param name="unitOfWorks"></param>
        /// <returns></returns>
        public static RelationalUnitOfWorkTransaction New(params IRelationalUnitOfWork[] unitOfWorks) =>
            new RelationalUnitOfWorkTransaction(unitOfWorks);
    }
}