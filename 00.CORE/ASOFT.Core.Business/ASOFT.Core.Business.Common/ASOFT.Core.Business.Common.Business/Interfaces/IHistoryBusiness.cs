// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/03/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business.Interfaces
{
    public interface IHistoryBusiness<H> where H : HistoryEntity, new()
    {
        /// <summary>
        /// Add lịch sử
        /// </summary>
        /// <typeparam name="L"></typeparam>
        /// <param name="entity">Dữ liệu được thêm mới hoặc cập nhật</param>
        /// <param name="divisionID"></param>
        /// <param name="table">Bảng của chính</param>
        /// <param name="per">Loại lịch sử cần lưu</param>
        /// <param name="userID">Người tạo</param>
        /// <param name="dt">Các ID của detail được Xóa</param>
        /// <param name="parentTable">id bảng cha</param>
        /// <param name="valueParent">giá trị khóa chính của bảng cha</param>
        /// <param name="historyChange">Những thay đổi của entity dùng khi Update entity</param>
        /// <param name="screenID"></param>
        /// <param name="tableID"></param>
        /// <param name="moduleID"></param>
        /// <param name="useTransaction"></param>
        /// <param name="refName"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        Task AddHistory<L>(L entity, string divisionID, string table, ASOFTPermission per, string userID, List<string> dt = null, string parentTable = null
             , string valueParent = null, List<string> historyChange = null, string screenID = null, string tableID = null, string moduleID = null, bool useTransaction = false, string refName = null);
    }
}
