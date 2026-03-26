using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class OOT9002
    {
        public Guid APK { get; set; }
        public Guid APKMaster { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public int? ScreenType { get; set; }
        public string? ScreenID { get; set; }
        public string? ScreenName { get; set; }
        public string? ModuleID { get; set; }

        public string? UrlCustom { get; set; }
        public string? Parameters { get; set; }

        public byte? DeleteFlag { get; set; }

        public DateTime? CreateDate { get; set; } = null;
        public string? CreateUserID { get; set; }

        public DateTime? LastModifyDate { get; set; } = null;
        public string? LastModifyUserID { get; set; }

        public DateTime? EffectDate { get; set; } = null;
        public DateTime? ExpiryDate { get; set; } = null;

        public byte? Disabled { get; set; }

        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }

        public int? ShowType { get; set; }
        public int? BusinessTypeID { get; set; }
        public int? MessageType { get; set; }
    }
}
