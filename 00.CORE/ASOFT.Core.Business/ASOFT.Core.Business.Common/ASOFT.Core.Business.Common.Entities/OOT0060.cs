// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    17/12/2020      Tấn Thành       Tạo mới
// #################################################################

using System;

namespace ASOFT.Core.Business.Common.Entities
{
    public class OOT0060
    {
        public const string COL_APK = "APK";
        public const string COL_DivisionID = "DivisionID";
        public const string COL_TaskHourDecimal = "TaskHourDecimal";
        public const string COL_VoucherStatus = "VoucherStatus";
        public const string COL_VoucherTaskSample = "VoucherTaskSample";
        public const string COL_VoucherStep = "VoucherStep";
        public const string COL_VoucherProcess = "VoucherProcess";
        public const string COL_VoucherProjectSample = "VoucherProjectSample";
        public const string COL_VoucherProject = "VoucherProject";
        public const string COL_VoucherTask = "VoucherTask";
        public const string COL_VoucherIssues = "VoucherIssues";
        public const string COL_VoucherRequest = "VoucherRequest";
        public const string COL_VoucherMilestone = "VoucherMilestone";
        public const string COL_VoucherRelease = "VoucherRelease";
        public const string COL_CreateDate = "CreateDate";
        public const string COL_CreateUserID = "CreateUserID";
        public const string COL_LastModifyDate = "LastModifyDate";
        public const string COL_LastModifyUserID = "LastModifyUserID";

        public Guid? APK { get; set; }
        public string DivisionID { get; set; }
        public int? TaskHourDecimal { get; set; }
        public string VoucherStatus { get; set; }
        public string VoucherTaskSample { get; set; }
        public string VoucherStep { get; set; }
        public string VoucherProcess { get; set; }
        public string VoucherProjectSample { get; set; }
        public string VoucherProject { get; set; }
        public string VoucherTask { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string LastModifyUserID { get; set; }
        public string VoucherIssues { get; set; }
        public string VoucherRequest { get; set; }
        public string VoucherMilestone { get; set; }
        public string VoucherRelease { get; set; }
        public string VoucherBooking { get; set; }
    }
}
