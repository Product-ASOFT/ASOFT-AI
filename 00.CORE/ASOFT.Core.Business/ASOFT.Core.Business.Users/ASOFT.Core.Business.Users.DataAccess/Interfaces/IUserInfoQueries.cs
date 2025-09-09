// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    19/02/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.A00.Entities.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Interfaces
{
    public interface IUserInfoQueries
    {
        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy] Created [19/02/2020]
        /// </history>
        Task<AT1103ViewModel> GetUserInfo(string userID, CancellationToken cancellationToken);

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
        Task<bool> UpdateLanguageByUser(string userID, string languageID, CancellationToken cancellationToken);

        /// <summary>
        /// Cập nhật Token cho user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="tokenBearer"></param>
        /// <returns></returns>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        Task<bool> UpdateTokenByUser(string userID, string tokenBearer, CancellationToken cancellationToken);

    }
}
