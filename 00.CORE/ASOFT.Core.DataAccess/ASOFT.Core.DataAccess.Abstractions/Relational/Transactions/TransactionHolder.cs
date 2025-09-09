using System.Data;
using System.Transactions;

namespace ASOFT.Core.DataAccess.Relational.Transactions
{
    /// <summary>
    /// Struct chứa thông tin của transaction
    /// </summary>
    public readonly struct TransactionHolder
    {
        private readonly IDbTransaction _transaction;
        private readonly TransactionScope _transactionScope;

        /// <summary>
        /// Có phải là transaction scope hay không.
        /// </summary>
        public bool IsTransactionScope => _transactionScope != null;

        /// <summary>
        /// Get <see cref="IDbTransaction"/> hoặc mặc định.
        /// </summary>
        /// <returns></returns>
        public IDbTransaction GetTransactionOrDefault() => _transaction;

        /// <summary>
        /// Lấy <see cref="TransactionScope"/> hoặc mặc định.
        /// </summary>
        /// <returns></returns>
        public TransactionScope GetTransactionScopeOrDefault() => _transactionScope;

        /// <summary>
        /// Constructor cho <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="transaction"></param>
        public TransactionHolder(IDbTransaction transaction)
        {
            _transaction = transaction;
            _transactionScope = null;
        }

        /// <summary>
        /// Constructor cho <see cref="TransactionScope"/>.
        /// </summary>
        /// <param name="transactionScope"></param>
        public TransactionHolder(TransactionScope transactionScope)
        {
            _transactionScope = transactionScope;
            _transaction = null;
        }
    }
}