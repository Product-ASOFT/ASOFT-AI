using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.DataAccess.Entities;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Queries
{
    public class HistoryQueries<H> : BusinessDataAccess, IHistoryQueries<H> where H : HistoryEntity
    {


        public HistoryQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private const string SQL_Insert = @"
        IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[{0}]') AND TYPE IN (N'U'))
        BEGIN
            INSERT INTO {0}(DivisionID, Description, RelatedToID, RelatedToTypeID, CreateDate, CreateUserID, StatusID, ScreenID, TableID) 
            VALUES (@DivisionID, @Description, @RelatedToID, @RelatedToTypeID, GETDATE(), @CreateUserID, @StatusID, @ScreenID, @TableID)
        END
        ";
        public async Task<int> InstallHistory(H history)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@DivisionID", history.DivisionID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@Description", history.Description, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@RelatedToID", history.RelatedToID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@RelatedToTypeID", history.RelatedToTypeID, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("@StatusID", history.StatusID, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("@CreateUserID", history.CreateUserID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@ScreenID", history.ScreenID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@TableID", history.TableID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                return await connection.ExecuteAsync(string.Format(SQL_Insert, history.GetType().Name), dynamicParameters);
            });
        }
    }
}
