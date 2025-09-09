using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class CRMF2030ViewModel
    {
        public int RowNum { get; set; }
        public int TotalRow { get; set; }
        public Guid APK { get; set; }
        public string DivisionID { get; set; }
        public string LeadID { get; set; }
        public string LeadName { get; set; }
        public string LeadMobile { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string LeadTypeID { get; set; }
        public string LeadTypeName { get; set; }
        public string LeadStatusID { get; set; }
        public string LeadStatusName { get; set; }
        public string LeadSourceID { get; set; }
        public string LeadSourceName { get; set; }
        public string AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public string JobID { get; set; }
        public bool? Disabled { get; set; }
        public bool? IsCommon { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastModifyUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public bool? IsConvert { get; set; }
        public string InheritAccountID { get; set; }
        public string CampaignName { get; set; }
        public string TradeMarkID { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string CompanyName { get; set; }
        public string Url { get; set; }
        public string keyContactHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return $"<a href=\"{this.Url}/ViewMasterDetail2/Index/CRM/CRMF2032?PK={APK}&Table=CRMT20301&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{LeadID}</a>";
                }
                return this.LeadID;
            }
        }
    }
}
