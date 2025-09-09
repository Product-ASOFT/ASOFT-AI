// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    19/02/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.Entities.ViewModels;
using ASOFT.Core.DataAccess;
using Dapper;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ASOFT.A00.DataAccess.Queries
{
    public class UserInfoQueries : BusinessDataAccess, IUserInfoQueries
    {
        public UserInfoQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private static readonly string SQL_GetUserInfo = @"SELECT A0.APK, A0.DivisionID, A0.EmployeeID, A0.FullName, A0.DepartmentID, A1.DepartmentName, A0.TeamID, A0.EmployeeTypeID,
                                                               A0.HireDate, A0.EndDate, A0.BirthDay, A0.Tel, A0.Address, A0.Fax, A0.Email, A0.IsUserID,
                                                               A0.Disabled, A0.CreateDate, A0.CreateUserID, A0.LastModifyUserID, A0.LastModifyDate,
                                                               A0.IsCommon, A0.SipID, A0.SipPassword, A0.GroupID, A0.Signature, A0.Image01ID, A0.DutyID ,A0.PermissionUser, 
	                                                           A0.Nationality, A0.IndentificationNo, A0.PassportNo, A0.BankAccountNumber, A0.Gender, A0.MarriedStatus
                                                        FROM AT1103 A0 With (NOLOCK) 
                                                        INNER JOIN AT1102 A1 ON A1.DepartmentID = A0.DepartmentID
                                                        WHERE EmployeeID = @EmployeeID";

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [19/02/2020]
        /// </history>
        public async Task<AT1103ViewModel> GetUserInfo (string userID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@EmployeeID", userID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<AT1103ViewModel>(SQL_GetUserInfo, dynamicParameters);
            }, cancellationToken);
        }

        private const string SQL_UPDATELANGUAGEBYUSER = @"
            UPDATE AT14051 SET LanguageID = @LanguageID WHERE UserID = @UserID
        ";

        /// <summary>
        /// Cập nhật LanguageID theo userID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="languageID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<bool> UpdateLanguageByUser(string userID, string languageID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@LanguageID", languageID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                var result = await connection.ExecuteAsync(SQL_UPDATELANGUAGEBYUSER, dynamicParameters);
                return result > 0;
            }, cancellationToken);
        }

        private const string SQL_UPDATEUSERTOKEN = @"
            UPDATE AT1103 SET TokenBearer = @TokenBearer WHERE UserID = @UserID
        ";

        /// <summary>
        /// Cập nhật Token cho user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="tokenBearer"></param>
        /// <returns></returns>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<bool> UpdateTokenByUser(string userID, string tokenBearer, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@TokenBearer", tokenBearer, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                var result = await connection.ExecuteAsync(SQL_UPDATEUSERTOKEN, dynamicParameters);
                return result > 0;
            }, cancellationToken);
        }
    }
}
