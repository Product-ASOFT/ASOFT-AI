// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;

namespace ASOFT.Core.Business.Common.Queries.ViewModels
{
    /// <summary>
    /// View model cho ghi chú
    /// </summary>
    public class CRMT90031ViewModel
    {
        public int? TotalRow { get; set; }
        public Guid? APK { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public int? NotesID { get; set; }
        public string NotesSubject { get; set; }
        public string Description { get; set; }
       // public Guid? RelatedToID { get; set; }
        public int? RelatedToTypeID_REL { get; set; }
    }
}
