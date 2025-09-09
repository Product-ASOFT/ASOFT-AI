using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities
{
    public class ST2111 : BaseEntity
    {
        [Required, MaxLength(100)]
        public string AgentCode { get; set; } = null!;

        [MaxLength(100)]
        public string? ModuleCode { get; set; }

        [Required]
        public string PromptContent { get; set; } = null!;

        public string TypePrompt { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int Version { get; set; }
    }
}