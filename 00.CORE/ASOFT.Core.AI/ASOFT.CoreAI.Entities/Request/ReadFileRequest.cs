using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities
{
    public class ReadFileRequest
    {
        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string? AgentCode { get; set; }
        public bool IsStreaming { get; set; }
        public string? Question { get; set; }
        public List<string>? FilePaths { get; set; }
        public List<string>? FileNames { get; set; }
        public BEMF2002DetailModel? BEMF2002Detail { get; set; } = null;
        public List<BEMT2001Model>? BEMT2001Models { get; set; } = null;
        public List<AttachFileModel>? AttachFiles { get; set; } = null;
        public string? TextContent { get; set; } = null;
    }
}