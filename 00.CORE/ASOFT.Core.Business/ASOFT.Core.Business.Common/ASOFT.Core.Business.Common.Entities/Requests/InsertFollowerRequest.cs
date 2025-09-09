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
    /// Class cammand thêm người theo dõi
    /// </summary>
    public class InsertFollowerRequest
    {
        /// <summary>
        /// APK của phiếu
        /// </summary>
        [Required]
        public Guid APKMaster { get; set; }
        [Required]
        [StringLength(50)]
        public string FollowerID { get; set; }
        [Required]
        [StringLength(50)]
        public string FollowerTableID { get; set; }
        [Required]
        [StringLength(50)]
        public string TableID { get; set; }
        [Required]
        [StringLength(50)]
        public string DivisionID { get; set; }
        [Required]
        [StringLength(50)]
        public string AssignUserColumnName { get; set; }
        [StringLength(50)]
        public string UserID { get; set; }
        [StringLength(50)]
        public string Type { get; set; }
        [StringLength(250)]
        public string Link { get; set; }
        [StringLength(50)]
        public string ModuleID { get; set; }
        [StringLength(50)]
        public string ID { get; set; }
    }
}
