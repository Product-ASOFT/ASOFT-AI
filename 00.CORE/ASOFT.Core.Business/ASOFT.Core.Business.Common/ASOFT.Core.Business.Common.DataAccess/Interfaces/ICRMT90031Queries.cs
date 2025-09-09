// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Common.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    public interface ICommentQueries
    {
        /// <summary>
        /// Lấy danh sách ghi chú
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<(int TotalRow, IEnumerable<CRMT90031ViewModel>)> GetCommentList(Guid apk, int page, int pageSize, CancellationToken cancellationToken = default);

    }
}
