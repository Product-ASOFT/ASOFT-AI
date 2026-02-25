using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities
{
    public class AgentRequest //: IRequest<(string AgentCode, string Result)>
    {
        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string? AgentCode { get; set; }
        public string? Module { get; set; }
        public string? Intent { get; set; }
        public Dictionary<string, string>? Metadata { get; set; } = null;
        public string Question { get; set; }
        public string? FeatureCode { get; set; }

        public List<string> PluginCodes = new List<string>();
        public List<JObject>? Items { get; set; }
        public Guid? ChatSessionID { get; set; }
        public bool IsStreaming { get; set; } = false;
        public List<string> Permisions { get; set; } = new List<string>();
        public List<string>? FilePaths { get; set; }
        public List<string>? FileNames { get; set; }
        public BEMF2000ViewModel? BEMF2000ViewModel { get; set; } = null;
        public List<BEMF2001ViewModel>? BEMF2001ViewModel { get; set; } = null;
        public string? RoleName { get; set; }
    }
}