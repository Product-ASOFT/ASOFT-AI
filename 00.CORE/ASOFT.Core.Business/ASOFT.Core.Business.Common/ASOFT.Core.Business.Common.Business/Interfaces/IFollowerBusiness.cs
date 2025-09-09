// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    02/11/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business.Interfaces
{
    public interface IFollowerBusiness
    {
        /// <summary>
        /// Trả ra danh sách người theo dõi
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="tableID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [10/11/2020]
        /// </history>
        Task<List<FollowerViewModel>> GetFollowersAsync(string apk, string tableID, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới dữ liệu người theo dõi 
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="userID"></param>
        /// <param name="listUserID"></param>
        /// <param name="tableID"></param>
        /// <param name="apkMaster"></param>
        /// <param name="followerTable"></param>
        /// <param name="relatedToTypeID"></param>
        /// <param name="isRemove"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [10/11/2020]
        /// </history>
        Task<bool> SaveFollower(string divisionID, string userID, List<string> listUserID, string tableID, string apkMaster, string followerTable, int relatedToTypeID, bool isRemove, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới người theo dõi có transaction
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [10/11/2020]
        /// </history>
        Task<Core.DataAccess.Entities.Result<bool, Core.API.Httpss.Errors.ErrorModelV2>> SaveFollowerWithTransaction(InsertFollowerRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới người theo dõi có transaction
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [10/11/2020]
        /// </history>
        Task<Result<bool, ErrorModelV2>> DeleteFollowerWithTransaction(RemoveFollowerRequest request, CancellationToken cancellationToken = default);

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
        Task<bool> CloneDataFollower(string APKMaster, string APKDestination, string tableFollower, CancellationToken cancellationToken = default);
    }
}
