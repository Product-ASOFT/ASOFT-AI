// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.Business.Common.Entities.Requests
{
    /// <summary>
    /// Command gỡ người theo dõi
    /// </summary>
    public class RemoveFollowerRequest 
    {
        [Required]
        public Guid APKMaster { get; set; }
        [Required]
        public string FollowerID { get; set; }
        [Required]
        public string DivisionID { get; set; }
        public string UserID { get; set; }
        [Required]
        public string TableID { get; set; }
        [Required]
        public string FollowerTableID { get; set; }
        [Required]
        public string AssignUserColumnName { get; set; }

    }
}
