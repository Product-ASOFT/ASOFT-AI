//using ASOFT.AP.Entity;
//using Dapper;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace ASOFT.Notification.Firebase.Repositories
//{
//    public class DeviceTokenGroupSubscriptionRepository
//    {
//        private readonly string _connectionString;

//        public DeviceTokenGroupSubscriptionRepository(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        /// <summary>
//        /// Câu truy vấn lấy danh sách nhóm đã tồn tại.
//        /// </summary>
//        private static readonly string SqlGetExistedGroups = @"
//            SELECT A17.GroupID
//            FROM APT0017 AS A17 WITH(NOLOCK)
//             INNER JOIN (
//	            SELECT X.Data.query('GroupID').value('.','VARCHAR(256)') AS GroupID
//	            FROM @Xml.nodes('/Data/Item') AS X(Data)
//             ) AS Groups ON A17.GroupID = Groups.GroupID
//            WHERE (A17.Disabled = 0 OR A17.Disabled IS NULL)
//            GROUP BY A17.GroupID";
//        public async Task<IEnumerable<string>> GetExistedGroupsAsync(IEnumerable<string> groupIDs,
//           CancellationToken cancellationToken = default(CancellationToken))
//        {

//            if (!groupIDs.Any())
//            {
//                return Enumerable.Empty<string>();
//            }

//            var rootElement = new XElement("Data");

//            foreach (var groupID in groupIDs)
//            {
//                if (!string.IsNullOrWhiteSpace(groupID))
//                {
//                    rootElement.Add(new XElement("Item",
//                        new XElement("GroupID", groupID)));
//                }
//            }

//            using (var connection = new SqlConnection(_connectionString))
//            {
//                connection.Open();

//                return await connection.QueryAsync<string>(SqlGetExistedGroups, new {
//                    Xml= rootElement.ToString()
//                });
//            }
      
//        }


//        private static readonly string SqlDeleteByApkToken = @"DELETE FROM APT0017 WHERE APKToken = @APKToken";
//        /// <summary>
//        /// Xóa danh sách token group subscription bằng apk của token.
//        /// </summary>
//        /// <param name="apkOfToken"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public async Task<int> DeleteByApkTokenAsync(Guid apkOfToken,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            try
//            {
//                using (var connection = new SqlConnection(_connectionString))
//                {
//                    connection.Open();

//                    using (var transaction = connection.BeginTransaction())
//                    {
//                        var affectedRows = await connection.ExecuteAsync(SqlDeleteByApkToken, new
//                        {
//                            APKToken = apkOfToken,
//                        }, transaction: transaction);

//                        transaction.Commit();

//                        return affectedRows;
//                    }
//                }
//            }
//            catch (Exception e)
//            {

//                throw e;
//            }
//        }
//        //=> await EntitySet().Where(m => m.APKToken == apkOfToken).BatchDeleteAsync(cancellationToken);


//        private static readonly string SqlIsGroupExistedAsync = @"SELECT * FROM APT0017 WHERE GroupID = @GroupID";
//        /// <summary>
//        /// Kiểm tra nhóm đã tồn tại.
//        /// </summary>
//        /// <param name="groupID"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public async Task<bool> IsGroupExistedAsync(string groupID,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                connection.Open();

//                return await connection.QueryFirstOrDefaultAsync<APT0017>(SqlIsGroupExistedAsync, new
//                {
//                    GroupID = groupID
//                }) != null;

//            }
//        }
//        // => await EntitySet().AsNoTracking().AnyAsync(m => m.GroupID == groupID, cancellationToken);

//        private static readonly string SqlAdd = @"INSERT INTO [dbo].[APT0017]
//                                                       ([APK]
//                                                       ,[APKToken]
//                                                       ,[GroupID]
//                                                       ,[SubscriptionARN]
//                                                       ,[Disabled]
//                                                       ,[CreateUserID]
//                                                       ,[LastModifyUserID]
//                                                       ,[CreateDate]
//                                                       ,[LastModifyDate])
//                                                 VALUES
//                                                       (@APK
//                                                       ,@APKToken
//                                                       ,@GroupID
//                                                       ,@SubscriptionARN
//                                                       ,@Disabled
//                                                       ,@CreateUserID
//                                                       ,@LastModifyUserID
//                                                       ,@CreateDate
//                                                       ,@LastModifyDate)";
//        public async Task<int> Add(APT0017 entity)
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                connection.Open();

//                using (var transaction = connection.BeginTransaction())
//                {
//                    var affectedRows = await connection.ExecuteAsync(SqlAdd, new
//                    {
//                        entity.APK,
//                        entity.CreateDate,
//                        entity.CreateUserID,
//                        entity.Disabled,
//                        entity.DivisionID,
//                        entity.LastModifyDate,
//                        entity.LastModifyUserID,
//                        entity.APKToken,
//                        entity.SubscriptionARN
//                    }, transaction: transaction);

//                    transaction.Commit();

//                    return affectedRows;
//                }
//            }
//        }
//    }
//}
