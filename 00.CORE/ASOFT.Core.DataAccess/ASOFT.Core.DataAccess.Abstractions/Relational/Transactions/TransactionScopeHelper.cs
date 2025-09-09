using System.Transactions;

namespace ASOFT.Core.DataAccess.Relational.Transactions
{
    /// <summary>
    /// Helper class for transaction scope.
    /// </summary>
    public static class TransactionScopeHelper
    {
        /// <summary>
        /// Create transaction scope with async flow.
        /// </summary>
        /// <returns></returns>
        public static TransactionScope CreateScopeAsyncFlow() =>
            new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }
}