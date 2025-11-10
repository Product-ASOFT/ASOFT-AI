using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class CRMF2050ViewModel
    {
        public int RowNum { get; set; }
        public int TotalRow { get; set; }
        public Guid APK { get; set; }
        public string DivisionID { get; set; }
        public string OpportunityID { get; set; }
        public string OpportunityName { get; set; }
        public string LeadName { get; set; }
        public string StageID { get; set; }
        public string StageName { get; set; }
        public string CampaignID { get; set; }
        public string AccountID { get; set; }
        public decimal? ExpectAmount { get; set; }
        public string PriorityID { get; set; }
        public string PriorityName { get; set; }
        public string CauseID { get; set; }
        public string Notes { get; set; }
        public string AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public string SourceID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public decimal? Rate { get; set; }
        public string NextActionID { get; set; }
        public string NextActionName { get; set; }
        public DateTime? NextActionDate { get; set; }
        public bool? Disabled { get; set; }
        public bool? IsCommon { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastModifyUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string AccountName { get; set; }
        public string StatusName { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
        public string OpportunityHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return $"<a href=\"{this.Url}/ViewMasterDetail2/Index/CRM/CRMF2052?PK={APK}&Table=CRMT20501&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{OpportunityID}</a>";
                }
                return this.OpportunityID;
            }
        }
    }
}
