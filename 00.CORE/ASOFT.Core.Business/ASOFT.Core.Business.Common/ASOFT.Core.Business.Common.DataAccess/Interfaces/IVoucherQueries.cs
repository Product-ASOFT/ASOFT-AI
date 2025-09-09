// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    17/12/2020      Tấn Thành       Tạo mới
// #################################################################

using ASOFT.Core.Business.Common.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    public interface IVoucherQueries
    {
        /// <summary>
        /// Lấy thiết lập sinh mã tự động cho các loại chứng từ nghiệp vụ OO
        /// </summary>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [17/12/2020]
        /// </history>
        Task<OOT0060> GetSettingVoucherNoByDivisionID(string divisionID, CancellationToken cancellationToken = default);              
    }
}
