using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Data.Common;

namespace ASOFT.Core.DataAccess.Extensions
{
    public static class SqlServerDbContextOptionsExtensions
    {
        /// <summary>
        /// Sử dụng ASOFT Sql server
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <param name="connectionString"></param>
        /// <param name="sqlServerOptionsAction"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseASOFTSqlServer([NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            Checker.NotNull(optionsBuilder, nameof(optionsBuilder));
            optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction);
            ConfigureASOFTSqlServerService(optionsBuilder);
            return optionsBuilder;
        }

        /// <summary>
        /// Sử dụng ASOFT Sql server
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <param name="connection"></param>
        /// <param name="sqlServerOptionsAction"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseASOFTSqlServer([NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            Checker.NotNull(optionsBuilder, nameof(optionsBuilder));
            optionsBuilder.UseSqlServer(connection, sqlServerOptionsAction);
            ConfigureASOFTSqlServerService(optionsBuilder);
            return optionsBuilder;
        }

        /// <summary>
        /// Sử dụng ASOFT Sql server
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="connectionString"></param>
        /// <param name="sqlServerOptionsAction"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseASOFTSqlServer<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder, [NotNull] string connectionString,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
            where TContext : DbContext
        {
            Checker.NotNull(optionsBuilder, nameof(optionsBuilder));
            optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction);
            ConfigureASOFTSqlServerService(optionsBuilder);
            return optionsBuilder;
        }

        /// <summary>
        /// Sử dụng ASOFT Sql server
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="connection"></param>
        /// <param name="sqlServerOptionsAction"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseASOFTSqlServer<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder, [NotNull] DbConnection connection,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
            where TContext : DbContext
        {
            Checker.NotNull(optionsBuilder, nameof(optionsBuilder));
            optionsBuilder.UseSqlServer(connection, sqlServerOptionsAction);
            ConfigureASOFTSqlServerService(optionsBuilder);
            return optionsBuilder;
        }

        private static void ConfigureASOFTSqlServerService(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WithNoLockSqlServerQuerySqlGeneratorFactory>();
        }
    }
}