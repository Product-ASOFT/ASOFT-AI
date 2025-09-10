// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    11/01/2021      Tấn Thành       Tạo mới
// #################################################################
using ASOFT.A00.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Interfaces
{
    public interface IASOFTCommonQueries
    {
        Task<ST2101> GetConfigST2101ByKey(int groupID, string keyName, CancellationToken cancellationToken = default);
    }
}
