using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Queries
{
    public class OAuthQueries : BusinessDataAccess, IOAuthQueries
    {
        public OAuthQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private const string SQL_GetHost = @"Select top 1 HostIP from IOTT1000";
        public async Task<string> GetHost(CancellationToken cancellationToken = default)
        {
            return await UseConnectionAsync(
              async connection =>
                  await connection.QueryFirstOrDefaultAsync<string>(SQL_GetHost), cancellationToken);
        }

        private const string SQL_AccountLinkingID = @"Select top 1 Subdomain from AT0015 where AccountLinkingID  = @Domain";
        public async Task<string> GetDomain(string domain, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Domain", domain, DbType.String, ParameterDirection.Input);
            return await UseConnectionAsync(
              async connection =>
                  await connection.QueryFirstOrDefaultAsync<string>(SQL_AccountLinkingID, dynamicParameters), cancellationToken);
        }

        private const string SQL_GetDomainByRefreshToken = @"Select top 1 Subdomain from AT0015 where RefreshToken  = @Domain";
        public async Task<string> GetDomainByRefreshToken(string refreshToken, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Domain", refreshToken, DbType.String, ParameterDirection.Input);
            return await UseConnectionAsync(
              async connection =>
                  await connection.QueryFirstOrDefaultAsync<string>(SQL_AccountLinkingID, dynamicParameters), cancellationToken);
        }
    }
}
