using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.Entities;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Queries
{
    public class ST2101Queries : BusinessDataAccess, IST2101Queries
    {
        public ST2101Queries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {

        }
        private static readonly string SqlUpdateZaloRefreshToken = @"
        UPDATE ST2101 
        SET [KeyValue] = @KeyValueRefresh, [LastModifyDate] = @LastModifyDate
        WHERE [GroupID] = 12 AND [KeyName] = 'zalo_refresh_token'
        UPDATE ST2101 
        SET [KeyValue] = @KeyValueAccess, [LastModifyDate] = @LastModifyDate
        WHERE [GroupID] = 12 AND [KeyName] = 'zalo_access_token'";
        /// <summary>
        /// cập nhật refresh token và access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="accessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateZaloRefreshToken(string refreshToken, string accessToken, CancellationToken cancellationToken = default)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                // String or binary data would be truncated.\r\nThe statement has been terminated
                dynamicParameters.Add("@KeyValueRefresh", refreshToken, DbType.AnsiString);
                dynamicParameters.Add("@KeyValueAccess", accessToken, DbType.AnsiString);
                dynamicParameters.Add("@LastModifyDate", DateTime.Now, DbType.DateTime);
                return await UseConnectionAsync<bool>(async connection =>
                {
                    var result = await connection.ExecuteAsync(SqlUpdateZaloRefreshToken, dynamicParameters);
                    return result > 0 ? true : false;
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static readonly string SqlGetKeyValue = @"SELECT * FROM ST2101 WHERE [GroupID] = @GroupID AND [KeyName] = @KeyName";
        /// <summary>
        /// lấy dữ liệu bảng ST2101
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ST2101> GetData(string groupID, string keyName, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@GroupID", groupID, DbType.AnsiString);
            dynamicParameters.Add("@KeyName", keyName, DbType.AnsiString);
            return await UseConnectionAsync<ST2101>(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<ST2101>(SqlGetKeyValue, dynamicParameters);
            }, cancellationToken);
        }
    }
}
