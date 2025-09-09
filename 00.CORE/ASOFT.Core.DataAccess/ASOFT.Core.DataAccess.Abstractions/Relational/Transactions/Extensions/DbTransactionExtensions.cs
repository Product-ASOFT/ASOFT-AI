using System;
using System.Data;

namespace ASOFT.Core.DataAccess.Relational.Transactions.Extensions
{
    /// <summary>
    /// Extension cho db transaction
    /// </summary>
    public static class DbTransactionExtensions
    {
        /// <summary>
        /// Try commit transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void TryCommit(this IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            try
            {
                transaction.Commit();
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Try roll back transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void TryRollback(this IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            try
            {
                transaction.Rollback();
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}