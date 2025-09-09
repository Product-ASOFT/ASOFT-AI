// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    public interface ICRMT90031Context : IBusinessContext<CRMT90031>
    {
        Task<CRMT90031> GetByAPK(Guid apk, string divisionID,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
