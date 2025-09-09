// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;

namespace ASOFT.CoreAI.Entities
{
    public class MilestoneViewModel
    {
        public int? TotalRow { get; set; }
        public Guid APK { get; set; }
        public string DivisionID { get; set; }
        public string MilestoneID { get; set; }
        public string MilestoneName { get; set; }
        public byte? PriorityID { get; set; }
        public string StatusID { get; set; }
        public DateTime? TimeRequest { get; set; }
        public DateTime? DeadlineRequest { get; set; }
        public string ProjectID { get; set; }
        public string AssignedToUserID { get; set; }
        public string TypeOfMilestone { get; set; }
        public string StatusName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string AssignedToUserName { get; set; }
        public string ProjectName { get; set; }
        public string PriorityName { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string TypeOfMilestoneName { get; set; }
        public byte? DeleteFlg { get; set; }
        public string RowNum { get; set; }
        public string Url { get; set; }
        public string TimeRequestFormatted => TimeRequest?.ToString("dd/MM/yyyy") ?? "Chưa có";
        public string DeadlineRequestFormatted => DeadlineRequest?.ToString("dd/MM/yyyy") ?? "Chưa có";
        public string MilestoneHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return $"<a href=\"{this.Url}/ViewMasterDetail2/Index/OO/OOF2192?PK={APK}&Table=OOT2190&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{MilestoneID}</a>";
                }
                return this.MilestoneID;
            }
        }
    }
}
