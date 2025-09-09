// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace ASOFT.CoreAI.Entities
{
    public class SimpleTaskInfo
    {
        public int TotalRow { get; set; }
        public Guid APK { get; set; }
        public string TaskTypeID { get; set; }
        public string TaskTypeName { get; set; }
        public string TaskID { get; set; }
        public string TaskName { get; set; }
        public int OutOfDeadline { get; set; }
        public byte? PriorityID { get; set; }
        public string PriorityName { get; set; }
        public string AssignedToUserID { get; set; }
        public string SupportUserID { get; set; }
        public string ReviewerUserID { get; set; }
        public string StatusID { get; set; }
        public string StatusName { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }
        public decimal? PercentProgress { get; set; }
        public string Color { get; set; }
        public string ReviewerUserName { get; set; }
        public string SupportUserName { get; set; }
        public string AssignedToUserName { get; set; }
        public string DivisionID { get; set; }
        public string ProjectName { get; set; }
        public string ProcessName { get; set; }
        public string StepName { get; set; }
        public int RowNum { get; set; }
        public string Url { get; set; }
        public string TaskHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return $"<a href=\"{this.Url}/ViewMasterDetail2/Index/OO/OOF2112?PK={APK}&Table=OOT2110&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{TaskID}</a>";
                }
                return this.TaskID;
            }
        }
        public string PlanStartDateFormat
        {
            get
            {
                return PlanStartDate?.ToString("dd/MM/yyyy");
            }
        }
        public string PlanEndDateFormat
        {
            get
            {
                return PlanEndDate?.ToString("dd/MM/yyyy");
            }
        }
        public string Progress
        {
            get
            {
                DateTime currentDate = DateTime.Now;
                if (!PlanEndDate.HasValue)
                    return "Chưa đặt hạn";

                // (1) Đã hoàn thành
                if (this.StatusID == "TTCV0003")
                {
                    return "Hoàn thành";
                }

                // (2) Chưa hoàn thành & trễ hạn
                if (currentDate.Date > PlanEndDate.Value.Date)
                    return "Trễ hạn";

                // (3) Sắp quá hạn (còn ≤ 2 ngày)
                double daysRemaining = (PlanEndDate.Value.Date - currentDate.Date).TotalDays;
                if (daysRemaining > 0 && daysRemaining <= 2)
                    return "Sắp quá hạn";

                // (4) Chưa đến hạn (chưa đến ngày bắt đầu)
                if (PlanStartDate.HasValue && currentDate.Date < PlanStartDate.Value.Date)
                    return "Chưa đến hạn";

                // (5) Đang trong hạn
                return "Đang trong hạn";
            }
        }
    }
}
