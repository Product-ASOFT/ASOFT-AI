using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.DataAccess;
using Dapper;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess
{
    public class UtilQueries : BusinessDataAccess, IUtilQueries
    {
        public UtilQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private readonly string SQL_GetEntity = @"Select * from {0} WITH (NOLOCK) Where {1} = '{2}'";
        public async Task<T> GetEntity<T> (string table, string id, string idValue)
        {
            var sql = string.Format(SQL_GetEntity, table, id, idValue);
            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql);
            });
        }
    }
}
