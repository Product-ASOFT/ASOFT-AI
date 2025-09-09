using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.CoreAI.Entities
{
    public class ST2161 : BaseEntity
    {
        [Required]
        public Guid ChatMessageID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [StringLength(100)]
        public string? FileType { get; set; }

        public string? FileUrl { get; set; }

        [Required]
        public byte[] FileData { get; set; } = null!;

        [Required]
        public DateTime UploadedAt { get; set; }

        [ForeignKey(nameof(ChatMessageID))]
        public virtual ST2141 ChatMessage { get; set; } = null!;
    }
}