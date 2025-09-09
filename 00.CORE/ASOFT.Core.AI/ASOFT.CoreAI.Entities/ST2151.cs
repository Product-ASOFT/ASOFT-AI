using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.CoreAI.Entities
{
    public class ST2151 : BaseEntity
    {
        [Required]
        public Guid ChatMessageID { get; set; }

        public string? ResponseText { get; set; }

        [StringLength(50)]
        public string? ResponseType { get; set; } // AI, Human, System

        public DateTime? ResponseTime { get; set; }

        [ForeignKey(nameof(ChatMessageID))]
        public virtual ST2141 ChatMessage { get; set; } = null!;
    }
}