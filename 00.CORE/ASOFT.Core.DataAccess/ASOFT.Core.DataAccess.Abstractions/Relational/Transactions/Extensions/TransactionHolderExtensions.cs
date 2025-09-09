using System.Data;
using System.Transactions;

namespace ASOFT.Core.DataAccess.Relational.Transactions.Extensions
{
    /// <summary>
    /// Extension cho transaction holder
    /// </summary>
    public static class TransactionHolderExtensions
    {
        /// <summary>
        /// Try get db connection
        /// </summary>
        /// <param name="transactionHolder"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static bool TryGetDbConnection(this TransactionHolder transactionHolder,
            out IDbTransaction transaction)
        {
            if (transactionHolder.IsTransactionScope)
            {
                transaction = null;
                return false;
            }

            transaction = transactionHolder.GetTransactionOrDefault();
            return transaction != null;
        }

        /// <summary>
        /// Try get transaction scope
        /// </summary>
        /// <param name="transactionHolder"></param>
        /// <param name="transactionScope"></param>
        /// <returns></returns>
        public static bool TryGetTransactionScope(this TransactionHolder transactionHolder,
            out TransactionScope transactionScope)
        {
            if (transactionHolder.IsTransactionScope)
            {
                transactionScope = transactionHolder.GetTransactionScopeOrDefault();
                return transactionScope != null;
            }

            transactionScope = null;
            return false;
        }
    }
}