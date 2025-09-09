using ASOFT.Core.DataAccess.Relational;
using System.Data;

namespace ASOFT.Core.DataAccess
{
    public abstract class AdminDataAccess : RelationalDataAccess
    {
        protected AdminDataAccess(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        protected AdminDataAccess(IDbConnection connection) : base(connection)
        {
        }

        protected AdminDataAccess(IDbTransaction transaction) : base(transaction)
        {
        }

        /// <summary>
        /// Db connection provider key
        /// </summary>
        /// <returns></returns>
        protected override string GetDbConnectionProviderKey() => CommonConnectionKeys.Admin;
    }
}
