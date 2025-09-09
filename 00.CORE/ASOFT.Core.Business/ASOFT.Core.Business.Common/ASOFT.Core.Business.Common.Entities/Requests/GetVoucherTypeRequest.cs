// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################



namespace ASOFT.Core.Business.Common.Entities.Requests
{
    /// <summary>
    /// class command lấy loại voucher
    /// </summary>
    public class GetVoucherTypeRequest
    {
        public string Type { get; set; }
        public string DivisionID { get; set; }
    }
}
