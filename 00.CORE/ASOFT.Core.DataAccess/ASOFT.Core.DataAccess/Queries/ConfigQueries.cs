using ASOFT.Core.DataAccess;
using Dapper;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Data.Core.Queries
{
    public class ConfigQueries : BusinessDataAccess, IConfigQueries
    {
        public ConfigQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private static readonly string SQLGetValue = @"Select KeyValue from ST2101 where GroupID = @GroupID and KeyName = @KeyName";
        
        /// <summary>
        /// Lấy giá trị thiết lập từ DB theo group và tên
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="keyName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetConfigValue(int groupID, string keyName, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@GroupID", groupID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@KeyName", keyName, DbType.String, ParameterDirection.Input);
            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<string>(SQLGetValue, dynamicParameters);
                
            }, cancellationToken);
        }
    }
}
