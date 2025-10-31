using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.CoreAI.Entities
{
    public class ST2133 : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public Guid ChatSessionID { get; set; }

        public string? Message { get; set; }

        public bool IsUserMessage { get; set; } = true;

        public DateTime? MessageTime { get; set; }

        [StringLength(50)]
        public string? TypeChat { get; set; }

        [StringLength(50)]
        public string? ModuleName { get; set; }

        [StringLength(100)]
        public string? AgentCode { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ChatSessionID))]
        public virtual ST2132 ChatSession { get; set; } = null!;

        public virtual ICollection<ST2134> ChatResponses { get; set; } = new List<ST2134>();

        public virtual ICollection<ST2135> ChatFiles { get; set; } = new List<ST2135>();
    }
}