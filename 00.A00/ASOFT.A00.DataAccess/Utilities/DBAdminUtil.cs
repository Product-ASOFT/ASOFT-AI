using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.Entity;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Utilities
{
    public class DBAdminUtil : AdminDataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public DBAdminUtil(IDbConnectionProvider dbConnectionProvider, ILoggerFactory logger, IConfiguration configuration) : base(dbConnectionProvider)
        {
            _configuration = Checker.NotNull(configuration, nameof(configuration));
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }

        public async Task<bool> RunScripts(string[] paths, string dbName)
        {
            await UseConnectionAsync(async connection =>
            {
                foreach (var path in paths)
                {
                    try
                    {
                        var db = @" use {0} 
                                    ";
                        string script = db + File.ReadAllText(path);
                        await connection.ExecuteAsync(script);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                }
            });

            return true;
        }

        private const string SQLRestoreDB = @" RESTORE DATABASE [{0}] FROM DISK = '{1}'
                                            With 
		                                            MOVE '{2}' TO '{4}{0}.mdf',
		                                            MOVE '{3}' TO '{4}{0}_log.ldf',
		                                            REPLACE, recovery";
        public async Task<bool> CreateDB(string dbName, string userName, string password, string companyName, string id, string mainURL, string mainAPIURL, string webPhysicalPath, string apiPhysicalPath, CancellationToken cancellationToken = default)
        {
            return await UseConnectionAsync(async connection =>
            {
                try
                {
                    var logicalName = _configuration["AutoRegister:AdminDBLogicalName"].ToString();
                    var logicalLogName = _configuration["AutoRegister:AdminDBLogicalName_Log"].ToString();
                    var LogicalDir = _configuration["AutoRegister:AdminDBLogicalDir"].ToString().Replace(@"\\", @"\");
                    var dirDemoDB = _configuration["AutoRegister:DirDemoDB_Admin"].ToString().Replace(@"\\", @"\");
                    await connection.ExecuteAsync(string.Format(SQLRestoreDB, dbName, dirDemoDB, logicalName, logicalLogName, LogicalDir + dbName.Replace("AS_ADMIN_", "") + @"\01.DATA\"), cancellationToken, commandTimeout: 600);
                }
                catch (Exception e)
                {
                    _logger.LogError($"AutoRegister Error at ID '{id}' while creating DB: " + e.ToString());
                    throw e;
                }
                return true;
            });
        }

        private const string SQL_GetScreens = @"SELECT S1.ScreenID as ItemKey,S1.ScreenID as Value, S1.ScreenName as DisplayName
			FROM sysScreen S1 WITH (NOLOCK)
				LEFT JOIN sysTable S2 WITH (NOLOCK) ON S1.sysTable = S2.TableName
			WHERE S1.ScreenType = '2'";

        private const string SQL_GetTypeRel = @"SELECT TypeRel from sysTable (NOLOCK) where TableName = @TableID";
        /// <summary>
        /// Hàm lấy typeRel của bảng sysTable (DB ADMIN)
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        /// <history>
        ///     [Hoài Thanh] created on [22/06/2022]
        /// </history>
        public async Task<string> GetTypeRel(string tableID)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@TableID", tableID, DbType.String, ParameterDirection.Input);
            return await UseConnectionAsync(async connection => {
                var result = await connection.QueryFirstOrDefaultAsync<string>(SQL_GetTypeRel, dynamicParameters);
                return result ?? null;
            });
        }

    }
}
