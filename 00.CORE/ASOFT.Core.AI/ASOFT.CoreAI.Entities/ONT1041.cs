using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class ONT1041
    {
        public Guid APK { get; set; } 
        public Guid APK_ONT1040 { get; set; }
        public string ParameterID { get; set; } = null!;
        public string ParameterName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public bool? IsUsed { get; set; }
        public int? OrdinalNumber { get; set; }
        public byte ParameterRole { get; set; }
        public string? NodeParent { get; set; }
        public string? CreateUserID { set; get; }
        public DateTime? CreateDate { set; get; }
        public string? LastModifyUserID { set; get; }
        public DateTime? LastModifyDate { set; get; }
    }
}
