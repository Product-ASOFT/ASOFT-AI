// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    20/08/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess;
using ASOFT.Core.Business.Users.DataAccsess.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ASOFT.Core.Business.Users.Entities.ViewModels;

namespace ASOFT.Core.Business.Users.DataAccsess.Queries
{
    public class ScreenPermissionQueries : BusinessDataAccess, IScreenPermissionQueries
    {
        public ScreenPermissionQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        { }

        /// <summary>
        /// Lấy dữ  liệu phân quyền màn hình APP
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="DivisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AP1403ViewModel>> GetScreenPermissionAsync(string userID, string DivisionID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", DivisionID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@IsApp", 1, DbType.Byte, ParameterDirection.Input);
            dynamicParameters.Add("@CustomerIndex", -1, DbType.Int32, ParameterDirection.Input);
            
            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<AP1403ViewModel>("AP1403", dynamicParameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        /// <summary>
        /// Lấy dữ liệu phân quyền màn hình ERPX
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="divisionID"></param>
        /// <param name="customerIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<IEnumerable<AP1403ViewModel>> GetERPXScreenPermissionAsync(string userID, string divisionID, int customerIndex, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", divisionID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@CustomerIndex", customerIndex, DbType.Int32, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<AP1403ViewModel>("AP1403", dynamicParameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        /// <summary>
        /// Lấy dữ  liệu phân quyền màn hình cho khách hàng dùng app "module CCM"
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="DivisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //private const string SQL_GetCustomerPermisson = @"select * from AT1403 where GroupID = 'ccm_permisson_groupid' and ModuleID = 'ASOFTCCM' and DivisionID=@DivisionID";
        public async Task<IEnumerable<AP1403ViewModel>> GetCustomerPermisson(string groupID, string DivisionID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@GroupID", groupID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", DivisionID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@IsApp", 1, DbType.Byte, ParameterDirection.Input);
            dynamicParameters.Add("@CustomerIndex", -1, DbType.Int32, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<AP1403ViewModel>("CCMP1403", dynamicParameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }
    }
}
