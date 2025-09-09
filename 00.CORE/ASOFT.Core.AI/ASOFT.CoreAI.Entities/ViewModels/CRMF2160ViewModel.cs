using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class CRMF2160ViewModel
    {
        public int RowNum { get; set; }
        public int TotalRow { get; set; }
        public string APK { get; set; }
        public string DivisionID { get; set; }
        public string SupportRequiredID { get; set; }
        public string SupportRequiredName { get; set; }
        public string TypeOfRequest { get; set; }
        public int PriorityID { get; set; }
        public string StatusID { get; set; }
        public DateTime? TimeRequest { get; set; }
        public DateTime? DeadlineRequest { get; set; }
        public string ReleaseVerion { get; set; } 
        public DateTime CreateDate { get; set; }
        public string AccountID { get; set; }
        public string ContactID { get; set; }
        public string InventoryID { get; set; }
        public string AssignedToUserID { get; set; }
        public string AccountName { get; set; }
        public string ContactName { get; set; }
        public string InventoryName { get; set; }
        public string PriorityName { get; set; }
        public string AssignedToUserName { get; set; }
        public string StatusName { get; set; }
        public string StatusQualityOfWork { get; set; }
        public string PriorityName1 { get; set; }
        public string Color { get; set; }
        public string TypeID { get; set; }
        public string Description { get; set; }
        public string IDInven { get; set; }
        public string TypeOfRequestID { get; set; }

        public string TimeRequestFormatted => TimeRequest?.ToString("dd/MM/yyyy") ?? "Chưa có";
        public string DeadlineRequestFormatted => DeadlineRequest?.ToString("dd/MM/yyyy") ?? "Chưa có";

        public string Url { get; set; }
        public string SupportRequiredHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return $"<a href=\"{this.Url}/ViewMasterDetail2/Index/CRM/CRMF2162?PK={APK}&Table=OOT2170&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{SupportRequiredID}</a>";
                }
                return this.SupportRequiredID;
            }
        }
    }
}
