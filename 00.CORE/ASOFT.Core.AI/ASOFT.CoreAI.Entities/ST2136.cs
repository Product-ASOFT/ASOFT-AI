using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class ST2136
    {
        public Guid APK { get; set; }
        public Guid APKMaster { get; set; }
        public string? BusinessParent { get; set; }
        public int? CriteriaID { get; set; }
        public string? CriteriaName { get; set; }
        public string? CriteriaStatus { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateUserID { get; set; }
        public string? LastModifyUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
