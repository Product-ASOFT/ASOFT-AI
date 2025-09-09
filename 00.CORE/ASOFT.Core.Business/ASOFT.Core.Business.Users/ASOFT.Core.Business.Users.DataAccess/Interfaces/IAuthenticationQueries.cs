using ASOFT.Core.Business.Users.Entities;
using ASOFT.Core.DataAccess.Relational;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.DataAccess.Interfaces
{
    public interface IAuthenticationQueries : IRelationDataExecutor
    {
        /// <summary>
        /// Lấy thông tin đăng nhập cho nhân viên (AT1405)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationUserInfo> GetAuthenticationUserInfoAsync(string userID, CancellationToken cancellationToken);

        // <summary>
        /// Lấy thông tin đăng nhập cho khách hàng (POST0011)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationUserInfo> GetAuthenticationCustomer(string userID, CancellationToken cancellationToken);

        /// <summary>
        /// Kiểm tra nếu người dùng tồn tại
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsUserExisted(string userID, CancellationToken cancellationToken);

        /// <summary>
        /// Lấy danh sách đơn vị theo mã người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<DivisionModel>> GetListDivisionByUserIdAsync(string userID,
            CancellationToken cancellationToken);

        /// <summary>
        /// Lấy đơn vị mặc định theo mã người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DivisionModel> GetDefaultDivisionByUserIdAsync(string userID,
            CancellationToken cancellationToken);

        /// <summary>
        /// Lấy thông tin user theo division
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationUserInfo> GetAuthenticationUserInfoByDivision(
            string userID, string divisionID, CancellationToken cancellationToken);

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="oldPassword"></param>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        Task<int> ChangePassword(string userID, string password,string oldPassword, string divisionID, string deviceID = null);

        /// <summary>
        /// Lấy thông tin userID khi ERP9.9 gọi qua
        /// Mục đích sinh ra hàm này là để lấy thông tin user khi login vào.
        /// Trường hợp có nhiều DivisionID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationUserInfo> GetAuthenticationUserInfoAsync_v2(string userID, CancellationToken cancellationToken);

        /// <summary>
        /// Lấy danh sách đơn vị theo mã người dùng
        /// Vì có trường hợp User thuộc @@@ nhưng bảng Đơn vị AT1101 không có tạo Đơn vị @@@
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<DivisionModel>> GetListDivisionByUserIdAsync_v2(string userID,
            CancellationToken cancellationToken);

        /// <summary>
        /// Lấy mã biometrics đăng nhập sinh trắc học
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationUserInfo> GetBiometricsKeyByUserId(string userID, CancellationToken cancellationToken);

        /// <summary>
        /// Cập nhật Biometrics Key
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="biometricsKey"></param>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        Task<int> UpdateBiometricsKey(string userID, string biometricsKey, string divisionID);

        /// <summary>
        /// Lấy dữ liệu kỳ đầu tiên
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        Task<string> GetFirstPeriod(string divisionID, CancellationToken cancellationToken);

        /// <summary>
        /// Lấy số lượng thông báo theo userID và divisionID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        Task<int> GetAmountNotification(string userID, string divisionID, CancellationToken cancellationToken);

        /// <summary>
        /// Lấy dữ liệu thiết lập mail
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        Task<string> GetMailSettingReceives(string divisionID, CancellationToken cancellationToken);
    }
}