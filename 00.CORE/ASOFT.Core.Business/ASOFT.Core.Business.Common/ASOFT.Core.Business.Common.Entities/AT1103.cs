// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess.Entities;
using System;

namespace ASOFT.Core.Business.Common.Entities
{
    public class AT1103 : CategoryEntity
    {
        public string EmployeeID { set; get; }
        public string FullName { set; get; }
        public string DepartmentID { set; get; }
        public string EmployeeTypeID { set; get; }
        public System.DateTime? HireDate { set; get; }
        public System.DateTime? EndDate { set; get; }
        public System.DateTime? BirthDay { set; get; }
        public string Address { set; get; }
        public string Tel { set; get; }
        public string Fax { set; get; }
        public string Email { set; get; }
        public byte? IsUserID { set; get; }
        public byte? IsCommon { set; get; }
        public string SipID { set; get; }
        public string SipPassword { set; get; }
        public string GroupID { set; get; }
        public string Signature { set; get; }
        public byte[] Image01ID { set; get; }
        public string DutyID { set; get; }
        public string PermissionUser { set; get; }
        public string Nationality { set; get; }
        public string IndentificationNo { set; get; }
        public string PassportNo { set; get; }
        public string BankAccountNumber { set; get; }
        public string Gender { set; get; }
        public string MarriedStatus { set; get; }
        public string TeamID { set; get; }
        public decimal? QuantityMax { set; get; }
        public string FunctionID { set; get; }
        public string EContractAccount { get; set; }
        public string EContractPassword { get; set; }
        public string EContractToken { get; set; }
        public string TokenBearer { get; set; }
        public DateTime? EContractExpTime { get; set; }
    }
}
