using ASOFT.Core.Business.Users.DataAccess.Interfaces;
using ASOFT.Core.Business.Users.Entities;
using ASOFT.Core.DataAccess;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.DataAccsess.Queries
{
    public class AuthenticationQueries : BusinessDataAccess, IAuthenticationQueries
    {
        /// <summary>
        /// Lấy thông tin người bằng UserID
        /// </summary>
        private static readonly string SqlGetUserByID = @"
            SELECT TOP(1) A1405.APK, A1402.DivisionID, A1405.UserID, A1405.UserName, A1405.Password,
                          A1405.PageSize, A1405.LanguageID, A1405.Disabled, A1402.GroupID, A1402.UserJoinRoleID, A1103.DepartmentID
            FROM AT1402 AS A1402 WITH(NOLOCK)
             INNER JOIN AT1405 AS A1405 WITH(NOLOCK) ON (A1402.DivisionID = A1405.DivisionID OR A1405.DivisionID = '@@@')
            										  AND A1402.UserID = A1405.UserID
            										  AND A1402.UserID = @UserID
            										  AND (A1405.Disabled = 0 OR A1405.Disabled IS NULL)
            										  AND (A1405.IsLock = 0 OR A1405.IsLock IS NULL) --and A1405.DivisionID not in ('@@@')
            INNER JOIN AT1103 AS A1103 ON A1103.DivisionID in (A1402.DivisionID, '@@@') AND A1103.EmployeeID = A1402.UserID
        ";
        private static readonly string SqlGetUserByIDAndDivision = @"
            SELECT TOP(1) A1405.APK, A1402.DivisionID, A1405.UserID, A1405.UserName, A1405.Password,
                          A1405.PageSize, A1405.LanguageID, A1405.Disabled, A1402.GroupID, A1402.UserJoinRoleID
            FROM AT1402 AS A1402 WITH(NOLOCK)
             INNER JOIN AT1405 AS A1405 WITH(NOLOCK) ON (A1405.DivisionID IN (@DivisionID, '@@@') AND A1402.DivisionID = @DivisionID)
            										  AND A1402.UserID = A1405.UserID
            										  AND A1402.UserID = @UserID
            										  AND (A1405.Disabled = 0 OR A1405.Disabled IS NULL)
            										  AND (A1405.IsLock = 0 OR A1405.IsLock IS NULL)";
        private static readonly string SqlCheckIsUserExisted =
            @"SELECT TOP 1 1 FROM AT1405 WITH(NOLOCK) WHERE UserID = @UserID";

        private static readonly string SqlDivisionListByUser =
            @"SELECT DISTINCT PermissionDivisionID AS DivisionID , AT1101.DivisionName AS DivisionName
                                                    FROM AT1411 WITH (NOLOCK) INNER JOIN AT1101 WITH (NOLOCK) ON AT1101.DivisionID = AT1411.PermissionDivisionID
                                                            LEFT JOIN AT1402  WITH (NOLOCK) ON AT1402.DivisionID = AT1411.DivisionID AND AT1402.GroupID = AT1411.GroupID
                                                    WHERE AT1402.UserID = @UserID ";

        private static readonly string SqlDefaultDivisionByUser =
            @"SELECT TOP 1 AT1101.DivisionID, AT1101.DivisionName
              FROM AT1402
              INNER JOIN AT1405 On AT1405.DivisionID IN ( AT1402.DivisionID, '@@@') AND  AT1405.UserID = AT1402.UserID
              INNER JOIN AT1101  WITH (NOLOCK) ON AT1101.DivisionID in (AT1402.DivisionID, '@@@') AND ISNULL(AT1101.Disabled, 0) = 0
              WHERE AT1402.UserID = @UserID";


        private static readonly string SqlGetCustomerByID = @"
            SELECT TOP(1) P1.APK, P1.DivisionID, P1.MemberID as UserID, P1.MemberName as UserName, P1.Password, IsNull(P1.DeliveryAddress, P1.Address) as  DeliveryAddress, P1.Email, P1.Tel, 1 as IsCustomer
            FROM POST0011 P1
            WHERE P1.MemberID = @UserID AND (P1.Disabled= 0 OR P1.Disabled IS NULL)";

        /// <summary>
        /// Lấy thông tin userID khi ERP9.9 gọi qua
        /// Mục đích sinh ra hàm này là để lấy thông tin user khi login vào.
        /// Trường hợp có nhiều DivisionID
        /// </summary>
        private static readonly string SqlGetUserByID_v2 = @"
            SELECT TOP (1) A1405.APK, A1405.DivisionID, A1405.UserID, A1405.UserName, A1405.Password,
                         A1405.PageSize, A1405.LanguageID, A1405.Disabled
            FROM AT1405 A1405 WITH (NOLOCK)
            WHERE UserID = @UserID AND (A1405.DivisionID IN (SELECT DISTINCT DivisionID FROM AT1101) OR A1405.DivisionID IN ('@@@'))";

        /// <summary>
        /// Lấy danh sách đơn vị theo mã người dùng
        /// Vì có trường hợp User thuộc @@@ nhưng bảng Đơn vị AT1101 không có tạo Đơn vị @@@
        /// </summary>
        private static readonly string SqlDivisionListByUser_v2 =
        @"SELECT DISTINCT A1.DivisionID, A1.DivisionName
		FROM AT1101 A1 WITH (NOLOCK)
			LEFT JOIN AT1405 A2 WITH (NOLOCK) ON A2.DivisionID IN (A1.DivisionID, '@@@')
		WHERE A2.UserID = @UserID AND A2.DivisionID IN (A1.DivisionID, '@@@')
		UNION ALL
		SELECT '@@@' AS DivisionID, N'Đơn vị dùng chung' AS DivisionName";

        private static readonly string SqlGetBiometricsKeyByID = @"
        SELECT TOP(1) A1405.APK, A1402.DivisionID, A1405.UserID, A1405.UserName, A1405.Password,
                      A1405.PageSize, A1405.LanguageID, A1405.Disabled, A1402.GroupID, A1402.UserJoinRoleID, A1405.BiometricsKey
        FROM AT1402 AS A1402 WITH(NOLOCK)
         INNER JOIN AT1405 AS A1405 WITH(NOLOCK) ON (A1402.DivisionID = A1405.DivisionID OR A1405.DivisionID = '@@@')
          										  AND A1402.UserID = A1405.UserID
          										  AND A1402.UserID = @UserID
          										  AND (A1405.Disabled = 0 OR A1405.Disabled IS NULL)
          										  AND (A1405.IsLock = 0 OR A1405.IsLock IS NULL)";

        public AuthenticationQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        /// <summary>
        /// Lấy thông tin đăng nhập cho nhân viên (AT1405)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthenticationUserInfo> GetAuthenticationUserInfoAsync(
            string userID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<AuthenticationUserInfo>(SqlGetUserByID,
                        dynamicParameters), cancellationToken);
        }

        // <summary>
        /// Lấy thông tin đăng nhập cho khách hàng (POST0011)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthenticationUserInfo> GetAuthenticationCustomer(
            string userID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<AuthenticationUserInfo>(SqlGetCustomerByID,
                        dynamicParameters), cancellationToken);
        }


        public async Task<AuthenticationUserInfo> GetAuthenticationUserInfoByDivision(
            string userID, string divisionID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", divisionID, DbType.AnsiString, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<AuthenticationUserInfo>(SqlGetUserByIDAndDivision,
                        dynamicParameters), cancellationToken);
        }

        public async Task<bool> IsUserExisted(string userID, CancellationToken cancellationToken)
        {
            return await UseConnectionAsync(async connection =>
            {
                var threshold = await connection.ExecuteScalarAsync<int?>(SqlCheckIsUserExisted, new {UserID = userID});
                return threshold == 1;
            }, cancellationToken);
        }

        public async Task<IEnumerable<DivisionModel>> GetListDivisionByUserIdAsync(string userID,
            CancellationToken cancellationToken)
        {
            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryAsync<DivisionModel>(SqlDivisionListByUser, new {UserID = userID}),
                cancellationToken);
        }

        public async Task<DivisionModel> GetDefaultDivisionByUserIdAsync(string userID,
            CancellationToken cancellationToken)
        {
            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<DivisionModel>(SqlDefaultDivisionByUser, new { UserID = userID }),
                cancellationToken);
        }

        /// <summary>
        /// Lấy danh sách đơn vị theo mã người dùng
        /// Vì có trường hợp User thuộc @@@ nhưng bảng Đơn vị AT1101 không có tạo Đơn vị @@@
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DivisionModel>> GetListDivisionByUserIdAsync_v2(string userID,
            CancellationToken cancellationToken)
        {
            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryAsync<DivisionModel>(SqlDivisionListByUser_v2, new { UserID = userID }),
                cancellationToken);
        }

        private static readonly string SqlChangePassword = @"UPDATE AT1405 SET [Password] = @Password
                                                            WHERE UserID = @UserID AND DivisionID = @DivisionID";
        private static readonly string SqlUpdateDeviceStatus = @"UPDATE AT0012 SET Status = 3
                                                            WHERE UserID = @UserID AND DivisionID = @DivisionID and DeviceID != @DeviceID and Status not in (1,2)";
        private static readonly string SqlUpdateLogoutStatus = @"UPDATE AT0012 SET Status = 1
                                                            WHERE UserID = @UserID AND DivisionID = @DivisionID and DeviceID = @DeviceID";
        private static readonly string SqlUpdateBiometricsKey = @"UPDATE AT1405 SET [BiometricsKey] = @BiometricsKey
                                                            WHERE UserID = @UserID AND DivisionID = @DivisionID";
        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="oldPassword"></param>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        public async Task<int> ChangePassword(string userID, string password, string oldPassword, string divisionID, string deviceID = null)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", divisionID, DbType.AnsiString, ParameterDirection.Input);
            dynamicParameters.Add("@Password", password, DbType.AnsiString, ParameterDirection.Input);
            dynamicParameters.Add("@OldPassword", oldPassword, DbType.AnsiString, ParameterDirection.Input);

            return await UseTransactionAsync(async (connection, holder) =>
            {
                var transaction = holder.GetTransactionOrDefault();
                int result = 0;
                try
                {
                    result = await connection.ExecuteAsync(SqlChangePassword, dynamicParameters, transaction);
                    if (string.IsNullOrEmpty(deviceID))
                    {
                        dynamicParameters.Add("@DeviceID", deviceID, DbType.String, ParameterDirection.Input);
                        await connection.ExecuteAsync(SqlUpdateDeviceStatus, dynamicParameters, transaction);
                        await connection.ExecuteAsync(SqlUpdateLogoutStatus, dynamicParameters, transaction);
                    }
                    transaction.Commit();

                }
                catch (System.Exception e)
                {
                    transaction.Rollback();
                    return -1;
                }
                return result;
            });
        }

        /// <summary>
        /// Lấy thông tin userID khi ERP9.9 gọi qua
        /// Mục đích sinh ra hàm này là để lấy thông tin user khi login vào.
        /// Trường hợp có nhiều DivisionID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthenticationUserInfo> GetAuthenticationUserInfoAsync_v2(
            string userID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<AuthenticationUserInfo>(SqlGetUserByID_v2,
                        dynamicParameters), cancellationToken);
        }

        /// <summary>
        /// Lấy mã biometrics đăng nhập sinh trắc học
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthenticationUserInfo> GetBiometricsKeyByUserId(
            string userID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<AuthenticationUserInfo>(SqlGetBiometricsKeyByID,
                        dynamicParameters), cancellationToken);
        }

        /// <summary>
        /// Cập nhật Biometrics Key
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="biometricsKey"></param>
        /// <returns></returns>
        public async Task<int> UpdateBiometricsKey(string userID, string biometricsKey, string divisionID)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.AnsiString, ParameterDirection.Input);
            dynamicParameters.Add("@BiometricsKey", biometricsKey, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", divisionID, DbType.AnsiString, ParameterDirection.Input);

            return await UseTransactionAsync(async (connection, holder) =>
            {
                var transaction = holder.GetTransactionOrDefault();
                int result = 0;
                try
                {
                    result = await connection.ExecuteAsync(SqlUpdateBiometricsKey, dynamicParameters, transaction);
                    transaction.Commit();

                }
                catch (System.Exception e)
                {
                    transaction.Rollback();
                    return -1;
                }
                return result;
            });
        }

        private const string SQL_TOP1PERIOD = @"
            SELECT TOP 1 * FROM HV9999 
            WHERE DivisionID = @DivisionID 
            ORDER BY TranYear, TranMonth
        ";


        /// <summary>
        /// Lấy dữ liệu kỳ đầu tiên
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<string> GetFirstPeriod(string divisionID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@DivisionID", divisionID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<string>(SQL_TOP1PERIOD, dynamicParameters), cancellationToken);
        }

        private const string SQL_Get_AmountNotification_By_User = @"
            SELECT COUNT(*)
            FROM OOT9002 O1 WITH (NOLOCK)
	            INNER JOIN OOT9003 O2 WITH (NOLOCK) ON O1.APK = O2.APKMaster
            WHERE O2.UserID = @UserID AND O2.DivisionID in (@DivisionID, '@@@') AND ISNULL(O2.DeleteFlg, 0) = 0 AND ISNULL(O1.[Disabled], 0) = 0
	            AND ISNULL(O2.IsRead, 0) = 0
        ";


        /// <summary>
        /// Lấy số lượng thông báo theo userID và divisionID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<int> GetAmountNotification(string userID, string divisionID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserID", userID, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@DivisionID", divisionID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<int>(SQL_Get_AmountNotification_By_User, dynamicParameters), cancellationToken);
        }

        private const string SQL_GETMAILSETTINGRECEIVES = @"
            SELECT SystemMailSettingReceives FROM AT0000 WITH (NOLOCK) WHERE DefDivisionID = @DivisionID
        ";

        /// <summary>
        /// Lấy dữ liệu thiết lập mail
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<string> GetMailSettingReceives(string divisionID, CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@DivisionID", divisionID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection =>
                    await connection.QueryFirstOrDefaultAsync<string>(SQL_GETMAILSETTINGRECEIVES, dynamicParameters), cancellationToken);
        }
    }
}