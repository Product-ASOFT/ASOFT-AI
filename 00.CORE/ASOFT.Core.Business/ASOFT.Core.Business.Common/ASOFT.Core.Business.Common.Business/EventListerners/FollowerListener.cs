// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using MediatR;
using System;

namespace ASOFT.Core.Business.Common.Business.EventListeners
{
    /// <summary>
    /// Class listener thêm người theo dõi
    /// </summary>
    public class FollowerListener : INotification
    {
        public Guid APKMaster { get; set; }
        public string FollowerID { get; set; }
        public string TableID { get; set; }
        public string CreateUserID { get; set; }
        public string DivisionID { get; set; }
        public string FollowerTable { get; set; }
    }
}
