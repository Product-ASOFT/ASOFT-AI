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
    public class CreateCommentRequest
    {
        [Required]
        public Guid APKMaster { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string NotesSubject { get; set; }
        public ScreenPermissionRequest PermissionInput { get; set; }
        public string Link { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string FollowerTable { get; set; }
        public string ModuleID { get; set; }
        public string TableID { get; set; }

    }
}
