// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    20/08/2020      Đoàn Duy      Tạo mới
// ##################################################################

namespace ASOFT.Core.Business.Users.Entities.ViewModels
{
    /// <summary>
    /// View model phân quyền màn hình
    /// </summary>
    public class AP1403ViewModel
    {
        public string ModuleID { get; set; }
        public string ScreenID { get; set; }
        public string DivisionID { get; set; }
        public byte IsAddNew { get; set; }
        public byte IsUpdate { get; set; }
        public byte IsView { get; set; }
        public byte IsDelete { get; set; }
        public byte IsPrint { get; set; }
        public byte IsExportExcel { get; set; }
        public byte IsHidden { get; set; }
    }
}
