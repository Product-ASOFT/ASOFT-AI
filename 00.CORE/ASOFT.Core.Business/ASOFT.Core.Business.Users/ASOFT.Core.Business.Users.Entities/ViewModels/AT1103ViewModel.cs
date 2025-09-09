// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    19/02/2021      Đoàn Duy      Tạo mới
// ##################################################################

using System;

namespace ASOFT.A00.Entities.ViewModels
{
    public class AT1103ViewModel
    {
        public Guid APK { get; set; }
        public string DivisionID { get; set; }
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }
}
