using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class HRMF1030ViewModel
    {
        public int RowNum { get; set; }
        public int? TotalRow { get; set; }

        public string? APK { get; set; }
        public string? DivisionID { get; set; }
        public string? CandidateID { get; set; }
                     
        public string? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
                     
        public string? DutyID { get; set; }
        public string? DutyName { get; set; }
                     
        public string? RecPeriodID { get; set; }
        public string? RecPeriodName { get; set; }
                     
        public string? RecruitStatus { get; set; }
        public string? Note { get; set; }

        public string? CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }

        public string? LastModifyUserID { get; set; }
        public DateTime LastModifyDate { get; set; }

        public string? CandidateName { get; set; }

        public string? GenderID { get; set; }    // mã giới tính (1, 0, …)
        public string? Gender { get; set; }      // Tên giới tính: Nam, Nữ
        public string Url { get; set; }

        public string CandidateHyperlinkedID
        {
            get
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    return $"<a href=\"{Url}/ViewMasterDetail2/Index/HRM/HRMF1032?PK={APK}&Table=HRMT1030&key=APK&DivisionID={DivisionID}\" target=\"_blank\">{CandidateID}</a>";
                }
                return CandidateID;
            }
        }
    }
}
