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
    public class OOT2160ViewModel
    {
        public int TotalRow { get; set; }
        public Guid APK { get; set; }
        public string IssuesID { get; set; }
        public string IssuesName { get; set; }
        public string PriorityID { get; set; }
        public string PriorityName { get; set; }
        public string StatusName { get; set; }
        public string StatusID { get; set; }
        public DateTime? TimeRequest { get; set; }
        public DateTime? DeadlineRequest { get; set; }
        public string ProjectName { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string TaskName { get; set; }
        public string TaskID { get; set; }
        public string AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public string ReleaseVerion { get; set; }
        public string InventoryID { get; set; }
        public string RequestID { get; set; }
        public string RequestSubject { get; set; }
        public string TypeOfIssues { get; set; }
        public string SupportRequiredID { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public byte? DeleteFlg { get; set; }
        public string InventoryName { get; set; }
        public string TypeOfIssuesName { get; set; }
        public int RowNum { get; set; }
        public string DivisionID { get; set; }
        public string ProjectID { get; set; }
        public string StatusQualityOfWork { get; set; }
        public string RequestName { get; set; }
        public string SupportRequiredName { get; set; }
        public DateTime? TimeConfirm { get; set; }
        public string CreateDateFormatted => CreateDate?.ToString("dd/MM/yyyy") ?? "Chưa có";
        public string TimeConfirmFormatted => TimeConfirm?.ToString("dd/MM/yyyy") ?? "Chưa xác nhận";
        public string Url { get; set; }
        public string IssuesHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return $"<a href=\"{this.Url}/ViewMasterDetail2/Index/OO/OOF2162?PK={APK}&Table=OOT2160&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{IssuesID}</a>";
                }
                return this.IssuesID;
            }
        }
    }
}
