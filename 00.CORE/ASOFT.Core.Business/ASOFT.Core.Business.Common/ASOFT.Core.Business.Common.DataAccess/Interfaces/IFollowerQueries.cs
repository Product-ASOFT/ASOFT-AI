using ASOFT.Core.Business.Common.Entities.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    public interface IFollowerQueries
    {
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
        Task<bool> InsertFollower(string followerTable, FollowerModel value, CancellationToken cancellationToken = default);

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
        Task<FollowerModel> GetDataByFollowerID(string followerID, string APKMaster, string followerTable, CancellationToken cancellationToken = default);

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
        Task<FollowerModel> GetData(string APKMaster, string followerTable, CancellationToken cancellationToken = default);

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
        Task<bool> DeleteFollowerByAPKMaster(string APKMaster, string followerID, string followerTable, CancellationToken cancellationToken = default);

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
        Task<bool> DeleteFollowerByAPKMaster(string APKMaster, string followerTable, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy ID người tạo và người phụ trách để lấy quyền thêm người theo dõi
        /// </summary>
        /// <param name="APKMaster"></param>
        /// <param name="tableID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [09/11/2020]
        /// </history>
        Task<IdsViewModel> GetIDs(string APKMaster, string tableID, string columnName, CancellationToken cancellationToken = default);

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
