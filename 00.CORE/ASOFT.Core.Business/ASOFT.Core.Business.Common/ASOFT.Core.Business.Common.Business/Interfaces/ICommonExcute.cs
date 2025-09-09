// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/03/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business.Interfaces
{
    /// <summary>
    /// Các luồng xử lý chuẩn
    /// </summary>
    /// <typeparam name="T">Class của nghiệp vụ</typeparam>
    /// <typeparam name="H">Class của bảng lịch sử</typeparam>
    public interface ICommonExcute<T, H> 
        where T : class
        where H : HistoryEntity, new()
    {
        /// <summary>
        /// Luồng insert nghiệp vụ không có detail
        /// </summary>
        /// <param name="entity">Đối tượng nghiệp vụ</param>
        /// <param name="pkName">Tên của trường ID dùng để check tồn tại</param>
        /// <param name="tableID">Tên bảng</param>
        /// <param name="userID"></param>
        /// <param name="languageID">Ngôn ngữ</param>
        /// <param name="voucherType">Loại voucher</param>
        /// <param name="followerList">Danh sách người theo dõi cần thêm</param>
        /// <param name="followerTb">Bảng người theo dõi</param>
        /// <param name="afterInsert">Xử lý sau insert</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        Task<Result<bool, ErrorModelV2>> InsertBusiness(T entity, string tableID, string userID, string pkName = null, string languageID = null, string voucherType = null,
            List<string> followerList = null, string followerTb = null, Func<Task> afterInsert = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Luồng insert nghiệp vụ không có detail
        /// </summary>
        /// <param name="pkValue"></param>
        /// <param name="pkName">Tên của trường ID dùng để check tồn tại</param>
        /// <param name="tableID">Tên bảng</param>
        /// <param name="userID"></param>
        /// <param name="screenID">mã màn hình để thêm lịch sử</param>
        /// <param name="moduleID">module dùng để lưu lịch sử</param>
        /// <param name="languageID">Ngôn ngữ</param>
        /// <param name="followerList">Danh sách người theo dõi cần thêm</param>
        /// <param name="followerTb">Bảng người theo dõi</param>
        /// <param name="refName"></param>
        /// <param name="beforeUpdate"></param>
        /// <param name="afterUpdate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        Task<Result<bool, ErrorModelV2>> UpdateBusiness(string pkValue, string pkName, string tableID, string userID, string screenID, string moduleID, string languageID = null,
            List<string> followerList = null, string followerTb = null, Func<T, Task> beforeUpdate = null, Func<T, Task> afterUpdate = null, CancellationToken cancellationToken = default, string refName = null);
    }
}
