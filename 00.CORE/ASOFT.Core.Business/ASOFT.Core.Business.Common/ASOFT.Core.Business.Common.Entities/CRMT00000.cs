// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;

namespace ASOFT.CRM.Entities
{
    public class CRMT00000
    {
        public Guid? APK { set; get; }
        public string DivisionID { set; get; }
        public int? TranYear { set; get; }
        public int? TranMonth { set; get; }
        public string VoucherType01 { set; get; }
        public string VoucherType02 { set; get; }
        public string VoucherType03 { set; get; }
        public string VoucherType04 { set; get; }
        public string VoucherType05 { set; get; }
        public string VoucherType06 { set; get; }
        public string WareHouseID { set; get; }
        public string WareHouseTempID { set; get; }
        public string ApportionID { set; get; }
        public string ExportAccountID { set; get; }
        public string ImportAccountID { set; get; }
        public DateTime? CreateDate { set; get; }
        public string CreateUserID { set; get; }
        public DateTime? LastModifyDate { set; get; }
        public string LastModifyUserID { set; get; }
        public string WareHouseBorrowID { set; get; }
        public string VoucherRequestCustomer { set; get; }
        public string VoucherContract { set; get; }
        public string VoucherRequestLicense { set; get; }
        public string VoucherRequestService { set; get; }
        public string VoucherSupportRequired { set; get; }
        public string VoucherSourceDataOnline { set; get; }
    }
}
