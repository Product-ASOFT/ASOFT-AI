using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class OOT9003
    {
        public Guid APK { get; set; }
        public Guid APKMaster { get; set; }

        public string? UserID { get; set; }
        public string? DivisionID { get; set; }

        public byte? IsRead { get; set; }
        public byte? DeleteFlg { get; set; }

        public string? CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }

        public string? LastModifyUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
