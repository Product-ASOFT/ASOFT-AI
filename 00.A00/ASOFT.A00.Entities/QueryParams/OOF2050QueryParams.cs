// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    01/02/2021      Đoàn Duy      Tạo mới
// #    06/11/2023      Ngô Dũng      Thêm trường lý do nghỉ
// ##################################################################


namespace ASOFT.A00.Entity.QueryParams
{
    public class OOF2050QueryParams
    {
        public string DivisionID { get; set; }
        public string UserID { get; set; }
        public int TranMonth { get; set; }
        public int TranYear { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string ID { get; set; }
        public string CreateUserID { get; set; }
        public string DepartmentID { get; set; }
        public string SectionID { get; set; }
        public byte Status { get; set; }
        public string Type { get; set; }
    }
}
