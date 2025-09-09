// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;

namespace ASOFT.Core.Business.Common.Entities.Requests
{
    /// <summary>
    /// Class gõ bỏ ghi chú
    /// </summary>
    public class RemoveCommentRequest 
    {
        public Guid? APK { get; set; }
        public string DivisionID { get; set; }
        public int? NotesID { get; set; }
        public string RelatedToID { get; set; }
        public string ModuleID { get; set; }
    }
}
