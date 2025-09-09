// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Files.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Files.DataAccess.Interfaces
{
    public interface IFileQueries
    {
        /// <summary>
        /// Lấy danh sách file đính kèm của nghiệp vụ.
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<(int TotalRow, IEnumerable<FileViewModel>)> GetAttachList(Guid apk, string divisionID, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
