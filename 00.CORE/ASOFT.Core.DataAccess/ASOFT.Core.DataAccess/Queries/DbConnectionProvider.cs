using ASOFT.Core.Common.InjectionChecker;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Base class cho connection provider
    /// </summary>
    public class DbConnectionProvider : IDbConnectionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IReadOnlyDictionary<string, string> ConnectionDictionary;

        /// <summary>
        /// Base connection provider
        /// </summary>
        /// <param name="connectionDictionary"></param>
        public DbConnectionProvider(IReadOnlyDictionary<string, string> connectionDictionary)
        {
            Checker.NotNull(connectionDictionary, nameof(connectionDictionary));
            ConnectionDictionary = connectionDictionary;
        }

        /// <summary>
        /// Provide instance of <see cref="IDbConnection"/>, etc <see cref="SqlConnection"/>.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns></returns>
        public IDbConnection ProvideDbConnection(string key)
        {
            Checker.NotNull(key, nameof(key));
            var connectionString = ProvideConnectionString(key);
            return GetSqlConnection(connectionString);
        }

        /// <summary>
        /// Function for getting <see cref="SqlConnection"/> from <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến Sql Server database.</param>
        /// <returns></returns>
        protected SqlConnection GetSqlConnection(string connectionString) =>
            new SqlConnection(connectionString);

        /// <inheritdoc />
        public string ProvideConnectionString(string key) => ConnectionDictionary[key];
    }
}