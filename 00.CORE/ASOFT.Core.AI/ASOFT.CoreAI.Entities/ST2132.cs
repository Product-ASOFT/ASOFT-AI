using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities
{
    public class ST2132 : BaseEntity
    {
        [StringLength(500)]
        public string SessionName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Status { get; set; }

        // Navigation property - một phiên chat có nhiều tin nhắn
        public virtual ICollection<ST2133> ChatMessages { get; set; } = new List<ST2133>();
    }
}