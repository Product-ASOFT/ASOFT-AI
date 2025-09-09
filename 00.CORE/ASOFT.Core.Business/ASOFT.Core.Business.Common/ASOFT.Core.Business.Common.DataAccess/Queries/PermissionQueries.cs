using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Common.Requests;
using ASOFT.Core.DataAccess;
using Dapper;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Helpers
{
    /// <summary>
    /// Permission helper
    /// </summary>
    public class PermissionQueries : BusinessDataAccess, IPermissionQueries
    {
        /// <summary>
        /// Helper class permission
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        public PermissionQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private readonly string SqlGetPermissionByScreen = @"SELECT AT1402.UserID, AT1403.ModuleID, AT1403.ScreenID, AT1403.DivisionID,  
                  MAX(IsAddNew) AS IsAddNew , 
                  MAX(IsUpdate) AS IsUpdate,
                  MAX(IsDelete) AS IsDelete,
                  MAX(IsView) AS IsView,
                  MAX(IsPrint) AS IsPrint,
                  MAX(IsExportExcel) AS IsExportExcel
                FROM AT1403
                LEFT JOIN AT1402 ON AT1402.DivisionID = AT1403.DivisionID AND AT1402.GroupID = AT1403.GroupID
                WHERE AT1403.DivisionID = @DivisionID AND AT1402.UserID = @UserID And ScreenID = @ScreenID
                GROUP BY AT1403.DivisionID, AT1403.ScreenID, AT1402.UserID, AT1403.ModuleID
                ORDER BY AT1402.UserID, AT1403.ModuleID, AT1403.ScreenID";

        public async Task<ScreenPermission> GetPermissionByScreenAsync(ScreenPermissionRequest @params, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@DivisionID", @params.DivisionID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@UserID", @params.UserID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@ScreenID", @params.ScreenID, DbType.String, ParameterDirection.Input);
            return await UseConnectionAsync(
             async connection =>
         await connection.QueryFirstOrDefaultAsync<ScreenPermission>(SqlGetPermissionByScreen, dynamicParameters), cancellationToken);
        }

        /// <summary>
        /// Lấy permission condition
        /// </summary>
        /// <param name="params"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PermissionCondition> GetPermissionConditionAsync(PermissionRequest @params,
            CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@DivisionID", @params.DivisionID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@ModuleID", PermissionConditions.FormatAsModuleID(@params.ModuleID), DbType.String,
                ParameterDirection.Input);
            dynamicParameters.Add("@DataID", @params.DataID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@DataType", @params.DataType, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@UserID", @params.UserID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@GroupID",
                string.IsNullOrWhiteSpace(@params.GroupID) ? string.Empty : @params.GroupID, DbType.String,
                ParameterDirection.Input);
            dynamicParameters.Add("@IsPrint", @params.IsPrint, DbType.Byte, ParameterDirection.Input);
            dynamicParameters.Add("@Permission", @params.Permission, DbType.Int32, ParameterDirection.Output);
            dynamicParameters.Add("@Condition", @params.Condition, DbType.String, ParameterDirection.Output,
                size: 4000);

            return await UseConnectionAsync(async connection =>
            {
                await connection.ExecuteAsync("AP1409", dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                return new PermissionCondition
                {
                    Permission = dynamicParameters.Get<int?>("@Permission").GetValueOrDefault(),
                    Condition = dynamicParameters.Get<string>("@Condition")
                };
            }, cancellationToken);
        }
    }
}