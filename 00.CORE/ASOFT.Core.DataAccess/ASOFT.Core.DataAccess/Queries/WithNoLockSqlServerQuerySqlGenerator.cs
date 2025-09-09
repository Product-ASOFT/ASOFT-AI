using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Sql server query generator for custom (WITH NOLOCK)
    /// </summary>
    public class WithNoLockSqlServerQuerySqlGenerator : SqlServerQuerySqlGenerator
    {
        private static readonly string WithNoLockHint = " WITH(NOLOCK)";

        /// <summary>
        /// Sql server query generator for custom (WITH NOLOCK)
        /// </summary>
        /// <param name="dependencies"></param>
        public WithNoLockSqlServerQuerySqlGenerator(
             QuerySqlGeneratorDependencies dependencies,
             IRelationalTypeMappingSource typeMappingSource,
             ISqlServerSingletonOptions sqlServerSingletonOptions)
             : base(dependencies, typeMappingSource, sqlServerSingletonOptions)
        {
        }

        /// <inheritdoc />
        protected override Expression VisitTable(TableExpression tableExpression)
        {
            Expression expression = base.VisitTable(tableExpression);
            Sql.Append(WithNoLockHint);
            return expression;
        }
    }
}