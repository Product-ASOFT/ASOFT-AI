using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Sql server generator factory custom with (WITH NOLOCK)
    /// </summary>
    public class WithNoLockSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _sqlGeneratorDependencies;
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISqlServerSingletonOptions _sqlServerSingletonOptions;

        /// <summary>
        /// Sql server generator factory custom with (WITH NOLOCK)
        /// </summary>
        public WithNoLockSqlServerQuerySqlGeneratorFactory(
            QuerySqlGeneratorDependencies sqlGeneratorDependencies,
            IRelationalTypeMappingSource typeMappingSource,
            ISqlServerSingletonOptions sqlServerSingletonOptions)
            : base(sqlGeneratorDependencies, typeMappingSource, sqlServerSingletonOptions)
        {
            _sqlGeneratorDependencies = sqlGeneratorDependencies;
            _typeMappingSource = typeMappingSource;
            _sqlServerSingletonOptions = sqlServerSingletonOptions;
        }

        /// <inheritdoc />
        public override QuerySqlGenerator Create()
        {
            return new WithNoLockSqlServerQuerySqlGenerator(
                _sqlGeneratorDependencies,
                _typeMappingSource,
                _sqlServerSingletonOptions);
        }
    }
}