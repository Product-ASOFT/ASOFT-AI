//####################################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.
//#
//# History:
//#     Date Time       Updater         Comment
//#     21/07/2020      Doand Duy        Tạo mới
//####################################################################

using ASOFT.A00.Entities;
using ASOFT.Notification.Domain.Aggregates;
using ASOFT.Notification.Firebase.Requests;
using ASOFT.Notification.Firebase.ViewModels;
using ASOFT.Notification.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ASOFT.Notification.Firebase.Repositories
{
    /// <summary>
    /// Repository cho việc truy cập dữ liệu của token thiết bị.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DiviceTokenFirebaseRepository : IDiviceTokenFirebaseRepository
    {
        private readonly string _connectionString;

        public DiviceTokenFirebaseRepository(string connectionString)
        {
            _connectionString = connectionString;

        }

        private static readonly string SqlGetTokenByUser = @"SELECT A12.NotifyToken,
                    (Select count(APK) from CCMT0005 WITH (NOLOCK) where IsRead = 0 and UserID = A12.UserID) As UnReadChat,
					(SELECT COUNT(*)
                      FROM OOT9002 O1 WITH (NOLOCK)
	                      INNER JOIN OOT9003 O2 WITH (NOLOCK) ON O1.APK = O2.APKMaster
                      WHERE O2.UserID = A12.UserID
	                      AND ISNULL(O2.IsRead, 0) = 0) as UnReadNoti
            FROM AT0012 AS A12 WITH(NOLOCK)
             INNER JOIN (
            	SELECT X.Data.query('UserID').value('.', 'NVARCHAR(25)') AS UserID
            	FROM @Xml.nodes('/Data/Item') AS X(Data)
             ) AS B16 ON A12.UserID = B16.UserID 
            WHERE A12.Status = 0";
        /// <summary>
        /// Lấy danh sách token từ danh sánh người dùng
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task<List<TokenInfo>> GetNotifyTokensByUser(List<string> users)
        {
            try
            {
                var rootElement = new XElement("Data");

                foreach (var user in users)
                {
                    if (!string.IsNullOrWhiteSpace(user))
                    {
                        rootElement.Add(new XElement("Item", new XElement("UserID", user)));
                    }
                }
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Xml", rootElement.ToString(), DbType.Xml, ParameterDirection.Input);
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<TokenInfo>(SqlGetTokenByUser, dynamicParameters);
                    return result.ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static readonly string SqlGetTokenByPlatforms = @"SELECT A12.NotifyToken
            FROM AT0012 AS A12 WITH(NOLOCK)
            WHERE A12.Status = 0 AND A12.DeviceOS in @Platforms AND UserID in @UserID";
        /// <summary>
        /// Lấy danh sách token từ danh sánh người dùng
        /// </summary>
        /// <param name="users"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public async Task<List<string>> GetNotifyTokensPlatforms(List<string> users, List<string> platforms)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserID", users);
                dynamicParameters.Add("@Platforms", platforms); 
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<string>(SqlGetTokenByPlatforms, dynamicParameters);
                    return result.ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        //private static readonly string SqlGetDeviceTokenByUserWithExcludeTokens = @"
        //    SELECT * FROM AT0012 AS A12 WITH(NOLOCK)
        //     LEFT JOIN (
        //    	SELECT X.Data.query('Token').value('.', 'NVARCHAR(250)') AS Token
        //    	FROM @Xml.nodes('/Data/Item') AS X(Data)
        //     ) AS B16 ON A12.Token = B16.Token
        //    WHERE B16.Token IS NULL AND A12.Status = 0";
        ///// <summary>
        ///// Lấy danh sách token thiết bị bằng mã người dùng
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="excludeDeviceTokens"></param>
        ///// <returns></returns>
        //public async Task<IEnumerable<AT0012>> GetListDeviceTokenByUser(string userID, IEnumerable<string> excludeDeviceTokens)
        //{
        //    var rootElement = new XElement("Data");

        //    foreach (var excludeDeviceToken in excludeDeviceTokens)
        //    {
        //        if (!string.IsNullOrWhiteSpace(excludeDeviceToken))
        //        {
        //            rootElement.Add(new XElement("Item",
        //                new XElement("Token", excludeDeviceToken)));
        //        }
        //    }
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        return await connection.QueryAsync<AT0012>(SqlGetDeviceTokenByUserWithExcludeTokens, new { Xml = rootElement.ToString() });
        //    }
        //}


        private static readonly string SqlInsert = @"INSERT INTO [dbo].[AT0012]
           ([APK],[DivisionID],[UserID],[DeviceID],[DeviceBrand],[DeviceName],[DeviceOS],[DeviceOSVersion],[Status],[DeviceToken],[NotifyToken]
           ,[FirstInstallTime],[LastedUpdateTime] ,[AppVersion],[LastIPAddress],[LastedLoginTime],[LastLogInfo],[LastLogWarn]
           ,[LastLogError],[Disable],[CreateUserID],[LastModifyUserID],[CreateDate],[LastModifyDate])
            VALUES
           (@APK
           ,@DivisionID
           ,@UserID
           ,@DeviceID
           ,@DeviceBrand
           ,@DeviceName
           ,@DeviceOS
           ,@DeviceOSVersion
           ,@Status
           ,@DeviceToken
           ,@NotifyToken
           ,@FirstInstallTime
           ,@LastedUpdateTime
           ,@AppVersion
           ,@LastIPAddress
           ,@LastedLoginTime
           ,@LastLogInfo
           ,@LastLogWarn
           ,@LastLogError
           ,@Disable
           ,@CreateUserID
           ,@LastModifyUserID
           ,@CreateDate
           ,@LastModifyDate)";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Add(AT0012 entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var affectedRows = await connection.ExecuteAsync(SqlInsert, new
                        {
                            entity.APK,
                            entity.AppVersion,
                            entity.CreateDate,
                            entity.CreateUserID,
                            entity.DeviceBrand,
                            entity.DeviceID,
                            entity.DeviceName,
                            entity.DeviceOSVersion,
                            entity.DeviceOS,
                            entity.DeviceToken,
                            entity.DivisionID,
                            entity.FirstInstallTime,
                            entity.LastedLoginTime,
                            entity.LastedUpdateTime,
                            entity.LastIPAddress,
                            entity.LastLogError,
                            entity.LastLogInfo,
                            entity.LastLogWarn,
                            entity.LastModifyDate,
                            entity.LastModifyUserID,
                            entity.NotifyToken,
                            entity.Status,
                            entity.UserID,
                            entity.Disable
                        }, transaction: transaction);

                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                }
            }
        }

        private static readonly string SqlUpdateDeviceInfo = @"UPDATE [dbo].[AT0012]
                                                                   SET [DeviceID] = @DeviceID
                                                                      ,[DeviceOSVersion] = @DeviceOSVersion
                                                                      ,[DivisionID] = @DivisionID
                                                                      ,[DeviceToken] = @DeviceToken
                                                                      ,[NotifyToken] = @NotifyToken
                                                                      ,[LastedUpdateTime] = @LastedUpdateTime
                                                                      ,[AppVersion] = @AppVersion
                                                                      ,[LastIPAddress] = @LastIPAddress
                                                                      ,[LastedLoginTime] = @LastedLoginTime
                                                                      ,[LastLogInfo] = @LastLogInfo
                                                                      ,[LastLogWarn] = @LastLogWarn
                                                                      ,[LastLogError] = @LastLogError
                                                                      ,[LastModifyUserID] = @LastModifyUserID
                                                                      ,[LastModifyDate] = @LastModifyDate
                                                                      ,[Disable] = @Disable
                                                                      ,[Status] = @Status
                                                                      ,[UserID] = @UserID
                                                                 WHERE APK = @APK";
        public async Task<int> UpdateDeviceInfo(RegisterDeviceTokenRequest request, AT0012 entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var AppVersion = request.AppVersion ?? entity.AppVersion;
                        var DeviceID = request.DeviceID ?? entity.DeviceID;
                        var DeviceOSVersion = request.DeviceOSVersion ?? entity.DeviceOSVersion;
                        var DeviceToken = request.DeviceToken ?? entity.DeviceToken;
                        var LastedLoginTime = request.LastedLoginTime ?? entity.LastedLoginTime;
                        var LastedUpdateTime = request.LastedUpdateTime ?? entity.LastedUpdateTime;
                        var LastIPAddress = request.LastIPAddress ?? entity.LastIPAddress;
                        var LastLogError = request.LastLogError ?? entity.LastLogError;
                        var LastLogInfo = request.LastLogInfo ?? entity.LastLogInfo;
                        var LastLogWarn = request.LastLogWarn ?? entity.LastLogWarn;
                        var Disable = request.Disable ?? entity.Disable;
                        var Status = request.Status ?? entity.Status;
                        var UserID = request.UserID ?? entity.UserID;
                        var DivisionID = request.DivisionID ?? entity.DivisionID;

                        var affectedRows = await connection.ExecuteAsync(SqlUpdateDeviceInfo, new
                        {
                            entity.APK,
                            DivisionID,
                            AppVersion,
                            DeviceID,
                            DeviceOSVersion,
                            DeviceToken,
                            LastedLoginTime,
                            LastedUpdateTime,
                            LastIPAddress,
                            LastLogError,
                            LastLogInfo,
                            LastLogWarn,
                            Disable,
                            Status,
                            LastModifyDate = DateTime.Now,
                            LastModifyUserID = request.UserID,
                            request.NotifyToken,
                            UserID
                        }, transaction: transaction);

                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                }
            }
        }

        private static readonly string SqlRemove = @"UPDATE [dbo].[AT0012]
                                                    SET NotifyToken = NULL, Status=@Status, LastLogInfo=@LastLogInfo WHERE APK = @APK and Status <> 2";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> RemoveNotifyToken(AT0012 entity)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var affectedRows = await connection.ExecuteAsync(SqlRemove, new
                        {
                            entity.APK,
                            entity.NotifyToken,
                            entity.LastLogInfo,
                            entity.Status
                        }, transaction: transaction);

                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception)
                    {

                        transaction.Rollback();
                        return 0;
                    }

                }
            }



        }

        private static readonly string SqlRemoveBulk = @"UPDATE AT0012 SET NotifyToken = NULL, Status=1, LastLogInfo=@LastLogInfo WHERE NotifyToken in @NotifyTokens and Status <> 2";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> RemoveBulkNotifyToken(List<string> tokens)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var affectedRows = await connection.ExecuteAsync(SqlRemoveBulk, new
                        {
                            NotifyTokens = tokens,
                            LastLogInfo = "invalid_notify_token"
                        }, transaction: transaction);

                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception)
                    {

                        transaction.Rollback();
                        return 0;
                    }

                }
            }
        }

        private static readonly string SqlGetToken = @"SELECT * FROM AT0012 WITH(NOLOCK) WHERE NotifyToken = @NotifyToken";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<AT0012> GetDeviceInfo(string token)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<AT0012>(SqlGetToken, new
                {
                    NotifyToken = token
                });

            }
        } 
        private static readonly string SqlGetUserToken = @"
                                                            SELECT A4.IsLock ,A1.* FROM AT0012 A1 WITH(NOLOCK)
                                                            left join AT1405 A4 on A1.UserID = A4.UserID and A1.DivisionID = A4.DivisionID
                                                            WHERE DeviceID = @DeviceID and DeviceOS = @DeviceOS";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="deviceOS"></param>
        /// <returns></returns>
        public async Task<AT0012ViewModel> GetDeviceInfo(string deviceID, string deviceOS)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    return await connection.QueryFirstOrDefaultAsync<AT0012ViewModel>(SqlGetUserToken, new
                    {
                        DeviceID = deviceID,
                        DeviceOS = deviceOS
                    });

                }
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }



        private static readonly string SqlUpdateDevicesStatusByUser = @"UPDATE [dbo].[AT0012]
                                                                   SET [Status] = @Status
                                                                    ,[LastModifyDate] = @LastModifyDate
                                                                 WHERE UserID = @UserID";
        public async Task<int> UpdateDevicesStatusByUser(string userID, int status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
 
                        var Status =status;
                        var UserID = userID;

                        var affectedRows = await connection.ExecuteAsync(SqlUpdateDevicesStatusByUser, new
                        {
                            Status,
                            LastModifyDate = DateTime.Now,
                            UserID
                        }, transaction: transaction);

                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                }
            }
        }

         private static readonly string SqlUpdateDevicesStatusByDevice = @"UPDATE [dbo].[AT0012]
                                                                   SET [Status] = @Status
                                                                    ,[LastModifyDate] = @LastModifyDate
                                                                 WHERE Device = @Device";
        public async Task<int> UpdateDevicesStatusByDevice(string deviceID, int status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
 
                        var Status =status;
                        var DeviceID = deviceID;

                        var affectedRows = await connection.ExecuteAsync(SqlUpdateDevicesStatusByUser, new
                        {
                            Status,
                            LastModifyDate = DateTime.Now,
                            DeviceID
                        }, transaction: transaction);

                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                }
            }
        }

    }
}
