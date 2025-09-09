using ASOFT.A00.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Interfaces
{
    public interface IST2101Queries
    {
        /// <summary>
        /// cập nhật refresh token và access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="accessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateZaloRefreshToken(string refreshToken, string accessToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// lấy dữ liệu bảng ST2101
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ST2101> GetData(string groupID, string keyName, CancellationToken cancellationToken = default);
    }
}
