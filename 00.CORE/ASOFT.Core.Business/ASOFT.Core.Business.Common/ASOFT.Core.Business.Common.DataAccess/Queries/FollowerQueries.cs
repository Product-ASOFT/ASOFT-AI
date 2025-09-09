// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    02/11/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Queries
{
    public class FollowerQueries : BusinessDataAccess, IFollowerQueries
    {
        public FollowerQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private const string SQL_GET_RELATEDTTID = @"
                INSERT INTO {0} (DivisionID, APKMaster, RelatedToID, TableID, FollowerID01, FollowerName01, FollowerID02, FollowerName02, FollowerID03
                                , FollowerName03, FollowerID04, FollowerName04, FollowerID05, FollowerName05, FollowerID06, FollowerName06, FollowerID07
                                , FollowerName07, FollowerID08, FollowerName08, FollowerID09, FollowerName09, FollowerID10, FollowerName10, FollowerID11
                                , FollowerName11, FollowerID12, FollowerName12, FollowerID13, FollowerName13, FollowerID14, FollowerName14, FollowerID15
                                , FollowerName15, FollowerID16, FollowerName16, FollowerID17, FollowerName17, FollowerID18, FollowerName18, FollowerID19
                                , FollowerName19, FollowerID20, FollowerName20, HashTags01, HashTags02, HashTags03, HashTags04, HashTags05, HashTags06
                                , HashTags07, HashTags08, HashTags09, HashTags10, TypeFollow, CreateDate, CreateUserID, RelatedToTypeID
                                , FollowerID21, FollowerID22, FollowerID23, FollowerID24, FollowerID25, FollowerID26, FollowerID27, FollowerID28, FollowerID29, FollowerID30 
                                , FollowerID31, FollowerID32, FollowerID33, FollowerID34, FollowerID35, FollowerID36, FollowerID37, FollowerID38, FollowerID39, FollowerID40 
                                , FollowerID41, FollowerID42, FollowerID43, FollowerID44, FollowerID45, FollowerID46, FollowerID47, FollowerID48, FollowerID49, FollowerID50
                                , FollowerName21, FollowerName22, FollowerName23, FollowerName24, FollowerName25, FollowerName26, FollowerName27, FollowerName28, FollowerName29, FollowerName30 
                                , FollowerName31, FollowerName32, FollowerName33, FollowerName34, FollowerName35, FollowerName36, FollowerName37, FollowerName38, FollowerName39, FollowerName40 
                                , FollowerName41, FollowerName42, FollowerName43, FollowerName44, FollowerName45, FollowerName46, FollowerName47, FollowerName48, FollowerName49, FollowerName50)
                VALUES (@DivisionID, @APKMaster, @RelatedToID, @TableID, @FollowerID01, @FollowerName01, @FollowerID02, @FollowerName02, @FollowerID03
                                , @FollowerName03, @FollowerID04, @FollowerName04, @FollowerID05, @FollowerName05, @FollowerID06, @FollowerName06, @FollowerID07
                                , @FollowerName07, @FollowerID08, @FollowerName08, @FollowerID09, @FollowerName09, @FollowerID10, @FollowerName10, @FollowerID11
                                , @FollowerName11, @FollowerID12, @FollowerName12, @FollowerID13, @FollowerName13, @FollowerID14, @FollowerName14, @FollowerID15
                                , @FollowerName15, @FollowerID16, @FollowerName16, @FollowerID17, @FollowerName17, @FollowerID18, @FollowerName18, @FollowerID19
                                , @FollowerName19, @FollowerID20, @FollowerName20, @HashTags01, @HashTags02, @HashTags03, @HashTags04, @HashTags05, @HashTags06
                                , @HashTags07, @HashTags08, @HashTags09, @HashTags10, @TypeFollow, @CreateDate, @CreateUserID, @RelatedToTypeID
                                , @FollowerID21, @FollowerID22, @FollowerID23, @FollowerID24, @FollowerID25, @FollowerID26, @FollowerID27, @FollowerID28, @FollowerID29, @FollowerID30 
                                , @FollowerID31, @FollowerID32, @FollowerID33, @FollowerID34, @FollowerID35, @FollowerID36, @FollowerID37, @FollowerID38, @FollowerID39, @FollowerID40 
                                , @FollowerID41, @FollowerID42, @FollowerID43, @FollowerID44, @FollowerID45, @FollowerID46, @FollowerID47, @FollowerID48, @FollowerID49, @FollowerID50
                                , @FollowerName21, @FollowerName22, @FollowerName23, @FollowerName24, @FollowerName25, @FollowerName26, @FollowerName27, @FollowerName28, @FollowerName29, @FollowerName30 
                                , @FollowerName31, @FollowerName32, @FollowerName33, @FollowerName34, @FollowerName35, @FollowerName36, @FollowerName37, @FollowerName38, @FollowerName39, @FollowerName40 
                                , @FollowerName41, @FollowerName42, @FollowerName43, @FollowerName44, @FollowerName45, @FollowerName46, @FollowerName47, @FollowerName48, @FollowerName49, @FollowerName50)";

        /// <summary>
        /// Thêm mới dữ liệu người theo dõi 
        /// </summary>
        /// <param name="followerTable"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [02/11/2020]
        /// </history>
        public async Task<bool> InsertFollower(string followerTable, FollowerModel value, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            AddParameter(dynamicParameters, value);
            return await UseConnectionAsync<bool>(async connection =>
            {
                var result = await connection.ExecuteAsync(string.Format(SQL_GET_RELATEDTTID, followerTable), dynamicParameters);
                return result > 0 ? true : false;
            }, cancellationToken);

        }

        /// <summary>
        /// Set parameter to DbCommand
        /// </summary>
        /// <param name="dynamicParameters"></param>
        /// <param name="value"></param>
        /// <param name="overrideValue"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đình Ly]   Created [24/12/2019]
        ///     [Tấn Thành] Edited [10/09/2020]
        /// </history>
        public void AddParameter(DynamicParameters dynamicParameters, FollowerModel value, bool overrideValue = false)
        {
            dynamicParameters.Add(FollowerModel.Column_DivisionID, value.DivisionID, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_APKMaster, value.APKMaster, DbType.Guid);
            dynamicParameters.Add(FollowerModel.Column_TableID, value.TableID, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_RelatedToID, value.RelatedToID, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID01, value.FollowerID01, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID02, value.FollowerID02, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID03, value.FollowerID03, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID04, value.FollowerID04, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID05, value.FollowerID05, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID06, value.FollowerID06, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID07, value.FollowerID07, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID08, value.FollowerID08, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID09, value.FollowerID09, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID10, value.FollowerID10, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID11, value.FollowerID11, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID12, value.FollowerID12, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID13, value.FollowerID13, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID14, value.FollowerID14, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID15, value.FollowerID15, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID16, value.FollowerID16, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID17, value.FollowerID17, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID18, value.FollowerID18, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID19, value.FollowerID19, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID20, value.FollowerID20, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName01, value.FollowerName01, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName02, value.FollowerName02, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName03, value.FollowerName03, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName04, value.FollowerName04, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName05, value.FollowerName05, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName06, value.FollowerName06, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName07, value.FollowerName07, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName08, value.FollowerName08, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName09, value.FollowerName09, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName10, value.FollowerName10, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName11, value.FollowerName11, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName12, value.FollowerName12, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName13, value.FollowerName13, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName14, value.FollowerName14, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName15, value.FollowerName15, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName16, value.FollowerName16, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName17, value.FollowerName17, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName18, value.FollowerName18, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName19, value.FollowerName19, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName20, value.FollowerName20, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags01, value.HashTags01, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags02, value.HashTags02, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags03, value.HashTags03, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags04, value.HashTags04, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags05, value.HashTags05, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags06, value.HashTags06, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags07, value.HashTags07, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags08, value.HashTags08, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags09, value.HashTags09, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_HashTags10, value.HashTags10, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_TypeFollow, value.TypeFollow, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_CreateDate, value.CreateDate, DbType.DateTime);
            dynamicParameters.Add(FollowerModel.Column_CreateUserID, value.CreateUserID, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_RelatedToTypeID, value.RelatedToTypeID, DbType.Int32);
            dynamicParameters.Add(FollowerModel.Column_FollowerID21, value.FollowerID21, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID22, value.FollowerID22, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID23, value.FollowerID23, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID24, value.FollowerID24, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID25, value.FollowerID25, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID26, value.FollowerID26, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID27, value.FollowerID27, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID28, value.FollowerID28, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID29, value.FollowerID29, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID30, value.FollowerID30, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID31, value.FollowerID31, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID32, value.FollowerID32, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID33, value.FollowerID33, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID34, value.FollowerID34, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID35, value.FollowerID35, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID36, value.FollowerID36, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID37, value.FollowerID37, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID38, value.FollowerID38, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID39, value.FollowerID39, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID40, value.FollowerID40, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID41, value.FollowerID41, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID42, value.FollowerID42, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID43, value.FollowerID43, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID44, value.FollowerID44, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID45, value.FollowerID45, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID46, value.FollowerID46, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID47, value.FollowerID47, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID48, value.FollowerID48, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID49, value.FollowerID49, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerID50, value.FollowerID50, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName21, value.FollowerName21, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName22, value.FollowerName22, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName23, value.FollowerName23, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName24, value.FollowerName24, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName25, value.FollowerName25, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName26, value.FollowerName26, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName27, value.FollowerName27, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName28, value.FollowerName28, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName29, value.FollowerName29, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName30, value.FollowerName30, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName31, value.FollowerName31, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName32, value.FollowerName32, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName33, value.FollowerName33, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName34, value.FollowerName34, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName35, value.FollowerName35, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName36, value.FollowerName36, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName37, value.FollowerName37, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName38, value.FollowerName38, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName39, value.FollowerName39, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName40, value.FollowerName40, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName41, value.FollowerName41, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName42, value.FollowerName42, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName43, value.FollowerName43, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName44, value.FollowerName44, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName45, value.FollowerName45, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName46, value.FollowerName46, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName47, value.FollowerName47, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName48, value.FollowerName48, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName49, value.FollowerName49, DbType.String);
            dynamicParameters.Add(FollowerModel.Column_FollowerName50, value.FollowerName50, DbType.String);
        }


        private const string SQL_GET_DATA_BY_FollowerID = @"
        SELECT APK, DivisionID, APKMaster, TableID, FollowerID01, FollowerName01, FollowerID02, FollowerName02, FollowerID03, FollowerName03
                , FollowerID04, FollowerName04, FollowerID05, FollowerName05, FollowerID06, FollowerName06, FollowerID07, FollowerName07
                , FollowerID08, FollowerName08, FollowerID09, FollowerName09, FollowerID10, FollowerName10, FollowerID11, FollowerName11
                , FollowerID12, FollowerName12, FollowerID13, FollowerName13, FollowerID14, FollowerName14, FollowerID15, FollowerName15
                , FollowerID16, FollowerName16, FollowerID17, FollowerName17, FollowerID18, FollowerName18, FollowerID19, FollowerName19
                , FollowerID20, FollowerName20, HashTags01, HashTags02, HashTags03, HashTags04, HashTags05, HashTags06, HashTags07, HashTags08
                , HashTags09, HashTags10,  TypeFollow, CreateDate, CreateUserID, RelatedToTypeID
        FROM {0} WITH (NOLOCK)
        WHERE (
                FollowerID01 = @FollowerID OR FollowerID02 = @FollowerID OR FollowerID03 = @FollowerID OR FollowerID04 = @FollowerID 
                OR FollowerID05 = @FollowerID OR FollowerID06 = @FollowerID OR FollowerID07 = @FollowerID OR FollowerID08 = @FollowerID 
                OR FollowerID09 = @FollowerID OR FollowerID10 = @FollowerID OR FollowerID11 = @FollowerID OR FollowerID12 = @FollowerID 
                OR FollowerID13 = @FollowerID OR FollowerID14 = @FollowerID OR FollowerID15 = @FollowerID OR FollowerID16 = @FollowerID 
                OR FollowerID17 = @FollowerID OR FollowerID18 = @FollowerID OR FollowerID19 = @FollowerID OR FollowerID20 = @FollowerID
                ) 
                AND CAST(APKMaster AS VARCHAR(50)) = @APKMaster";

        /// <summary>
        /// Get data bằng userID
        /// </summary>
        /// <param name="followerID"></param>
        /// <param name="APKMaster"></param>
        /// <param name="followerTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [09/11/2020]
        /// </history>
        public async Task<FollowerModel> GetDataByFollowerID(string followerID, string APKMaster, string followerTable, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@FollowerID", followerID, DbType.String);
            dynamicParameters.Add("@APKMaster", APKMaster, DbType.String);

            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<FollowerModel>(string.Format(SQL_GET_DATA_BY_FollowerID, followerTable), dynamicParameters);
            }, cancellationToken);
        }

        private const string SQL_GETDATA_BY_APKMASTER = @"
SELECT C1.APK, C1.DivisionID, C1.APKMaster, C1.RelatedToID, C1.TableID,
C1.FollowerID01, C1.FollowerID02, C1.FollowerID03, C1.FollowerID04, C1.FollowerID05, C1.FollowerID06, C1.FollowerID07, C1.FollowerID08, C1.FollowerID09, C1.FollowerID10,
C1.FollowerID11, C1.FollowerID12, C1.FollowerID13, C1.FollowerID14, C1.FollowerID15, C1.FollowerID16, C1.FollowerID17, C1.FollowerID18, C1.FollowerID19, C1.FollowerID20,
C1.FollowerID21, C1.FollowerID22, C1.FollowerID23, C1.FollowerID24, C1.FollowerID25, C1.FollowerID26, C1.FollowerID27, C1.FollowerID28, C1.FollowerID29, C1.FollowerID30, 
C1.FollowerID31, C1.FollowerID32, C1.FollowerID33, C1.FollowerID34, C1.FollowerID35, C1.FollowerID36, C1.FollowerID37, C1.FollowerID38, C1.FollowerID39, C1.FollowerID40, 
C1.FollowerID41, C1.FollowerID42, C1.FollowerID43, C1.FollowerID44, C1.FollowerID45, C1.FollowerID46, C1.FollowerID47, C1.FollowerID48, C1.FollowerID49, C1.FollowerID50,
C1.FollowerName01, C1.FollowerName02, C1.FollowerName03, C1.FollowerName04, C1.FollowerName05, C1.FollowerName06, C1.FollowerName07, C1.FollowerName08, C1.FollowerName09, C1.FollowerName10, 
C1.FollowerName11, C1.FollowerName12, C1.FollowerName13, C1.FollowerName14, C1.FollowerName15, C1.FollowerName16, C1.FollowerName17, C1.FollowerName18, C1.FollowerName19, C1.FollowerName20, 
C1.FollowerName21, C1.FollowerName22, C1.FollowerName23, C1.FollowerName24, C1.FollowerName25, C1.FollowerName26, C1.FollowerName27, C1.FollowerName28, C1.FollowerName29, C1.FollowerName30, 
C1.FollowerName31, C1.FollowerName32, C1.FollowerName33, C1.FollowerName34, C1.FollowerName35, C1.FollowerName36, C1.FollowerName37, C1.FollowerName38, C1.FollowerName39, C1.FollowerName40, 
C1.FollowerName41, C1.FollowerName42, C1.FollowerName43, C1.FollowerName44, C1.FollowerName45, C1.FollowerName46, C1.FollowerName47, C1.FollowerName48, C1.FollowerName49, C1.FollowerName50, 
C1.TypeFollow, C1.CreateDate, C1.RelatedToTypeID, C1.CreateUserID 
FROM {0} C1 WITH (NOLOCK)
	--INNER JOIN AT1103 A1 WITH (NOLOCK) ON A1.EmployeeID = C1.CreateUserID
WHERE CAST(C1.APKMaster AS VARCHAR(50)) = @APKMaster";

        /// <summary>
        /// Get danh sách người theo dõi 
        /// </summary>
        /// <param name="APKMaster"></param>
        /// <param name="followerTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [09/11/2020]
        /// </history>
        public async Task<FollowerModel> GetData(string APKMaster, string followerTable, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@APKMaster", APKMaster, DbType.String);
            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<FollowerModel>(string.Format(SQL_GETDATA_BY_APKMASTER, followerTable), dynamicParameters);
            }, cancellationToken);
        }

        // Query xóa Follower động theo Module 
        private const string SQL_DELETE_USERID_BY_APKMASTER = @"DELETE {0} WHERE FollowerID = @FollowerID AND CAST(APKMaster AS VARCHAR(50)) = @APKMaster";

        /// <summary>
        /// Xóa người theo dõi theo APKMaster
        /// </summary>
        /// <param name="APKMaster"></param>
        /// <param name="followerID"></param>
        /// <param name="followerTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [09/11/2020]
        /// </history>
        public async Task<bool> DeleteFollowerByAPKMaster(string APKMaster, string followerID, string followerTable, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@FollowerID", followerID, DbType.String);
            dynamicParameters.Add("@APKMaster", APKMaster, DbType.String);
            return await UseConnectionAsync<bool>(async connection =>
            {
                var result = await connection.ExecuteAsync(string.Format(SQL_DELETE_USERID_BY_APKMASTER, followerTable), dynamicParameters);
                return result > 0 ? true : false;
            }, cancellationToken);
        }

        private const string SQL_DELETE_FOLLOWER = @"DELETE FROM {0} WHERE APKMaster = @APKMaster";

        /// <summary>
        /// Xóa công việc, dự án trong bảng người theo dõi {X}T9020
        /// </summary>
        /// <param name="APKMaster"></param>
        /// <param name="followerTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [09/11/2020]
        /// </history>
        public async Task<bool> DeleteFollowerByAPKMaster(string APKMaster, string followerTable, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@APKMaster", APKMaster, DbType.String);
            return await UseConnectionAsync<bool>(async connection =>
            {
                var result = await connection.ExecuteAsync(string.Format(SQL_DELETE_FOLLOWER, followerTable), dynamicParameters);
                return result > 0 ? true : false;
            }, cancellationToken);
        }

        private static readonly string SqlGetIDs = @"Select CreateUserID ,{0} As AssignUserID From {1} Where APK = @APK";
        /// <summary>
        /// Lấy ID người tạo và người phụ trách để lấy quyền thêm người theo dõi
        /// </summary>
        /// <param name="APKMaster"></param>
        /// <param name="tableID"></param>
        /// <param name="columnName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [09/11/2020]
        /// </history>
        public async Task<IdsViewModel> GetIDs(string APKMaster, string tableID, string columnName, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@APK", APKMaster, DbType.String);
            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<IdsViewModel>(string.Format(SqlGetIDs, columnName, tableID), dynamicParameters);
            }, cancellationToken);
        }

        private const string SQL_CLONE_FOLLOWER = @"
            INSERT INTO OOT9020
            SELECT NEWID(), C1.DivisionID, @APKDestination, C1.RelatedToID, C1.TableID
                    , FollowerID01, FollowerName01, FollowerID02, FollowerName02, FollowerID03, FollowerName03, FollowerID04, FollowerName04, FollowerID05, FollowerName05
                    , FollowerID06, FollowerName06, FollowerID07, FollowerName07, FollowerID08, FollowerName08, FollowerID09, FollowerName09, FollowerID10, FollowerName10
                    , FollowerID11, FollowerName11, FollowerID12, FollowerName12, FollowerID13, FollowerName13, FollowerID14, FollowerName14, FollowerID15, FollowerName15
                    , FollowerID16, FollowerName16, FollowerID17, FollowerName17, FollowerID18, FollowerName18, FollowerID19, FollowerName19, FollowerID20, FollowerName20
                    , HashTags01, HashTags02, HashTags03, HashTags04, HashTags05, HashTags06, HashTags07, HashTags08, HashTags09, HashTags10
                    , TypeFollow, GETDATE(), CreateUserID, RelatedToTypeID
            FROM OOT9020 C1 WITH (NOLOCK)
            WHERE CONVERT(VARCHAR(50), APKMaster) = @APKMaster";

        /// <summary>
        ///     Sao chép dữ liệu Follower theo APKMaster
        /// </summary>
        /// <param name="APKMaster"></param>
        /// <param name="APKDestination"></param>
        /// <param name="tableFollower"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành] Tạo mới [17/12/2020]
        /// </history>
        public async Task<bool> CloneDataFollower(string APKMaster, string APKDestination, string tableFollower, CancellationToken cancellationToken = default)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("APKMaster", APKMaster, DbType.String, ParameterDirection.Input);
                dynamicParameters.Add("APKDestination", APKDestination, DbType.String, ParameterDirection.Input);
                return await UseConnectionAsync<bool>(async connection =>
                {
                    var result = await connection.ExecuteAsync(SQL_CLONE_FOLLOWER, dynamicParameters);
                    return result > 0 ? true : false;
                }, cancellationToken);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            throw new System.NotImplementedException();
        }
    }
}
