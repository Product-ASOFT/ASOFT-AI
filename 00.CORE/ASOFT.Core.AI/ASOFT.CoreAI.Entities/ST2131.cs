using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities
{
    public class ST2131 : BaseEntity
    {
        [StringLength(500)]
        public string SessionName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Status { get; set; }

        // Navigation property - một phiên chat có nhiều tin nhắn
        public virtual ICollection<ST2141> ChatMessages { get; set; } = new List<ST2141>();
    }
}