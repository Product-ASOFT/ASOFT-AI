using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities
{
    public class ReadFileRequest
    {
        [Required]
        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;
        public string? AgentCode { get; set; }
        public bool IsStreaming { get; set; }
        public string? Question { get; set; }
        public List<string>? FilePaths { get; set; }
        public List<string>? FileNames { get; set; }
        public BEMF2000ViewModel? BEMF2000ViewModel { get; set; } = null;
        public List<BEMF2001ViewModel>? BEMF2001ViewModels { get; set; } = null;
        public List<AttachFileModel>? AttachFiles { get; set; } = null;
        public string? TextContent { get; set; } = null;
        public OOT9002? OOT9002 { get; set; }
        public List<OOT9003>? OOT9003s { get; set; }
    }
}