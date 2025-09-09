// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    31/12/2020      Tấn Thành       Tạo mới
// #################################################################

using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business.Interfaces
{
    /// <summary>
    /// Giúp tạo voucher.
    /// </summary>
    public interface IVoucherBusiness
    {
        /// <summary>
        /// Tạo voucher no.
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> CreateVoucherAsync(VoucherInfo voucherInfo, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật voucher no.
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="voucherNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateVoucherAsync(VoucherInfo voucherInfo, string voucherNo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy thiết lập sinh mã tự động cho các loại chứng từ nghiệp vụ
        /// </summary>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành] Tạo mới [17/12/2019]
        /// </history>
        Task<OOT0060> GetSettingVoucherNoByDivisionID(string divisionID, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy voucher
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetVoucherNo(GetVoucherTypeRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Cập nhật voucher no.
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="voucherNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateVoucherAsyncNoTransaction(VoucherInfo voucherInfo, string voucherNo,
            CancellationToken cancellationToken = default);
    }
}