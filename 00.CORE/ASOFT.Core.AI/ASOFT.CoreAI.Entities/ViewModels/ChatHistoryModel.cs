namespace ASOFT.CoreAI.Entities
{
    public class ChatHistoryModel
    {
        public Guid? ChatSessionID { get; set; }
        public string UserID { get; set; } = null!;
        public string Question { get; set; } = null!;
        public string? TypeChat { get; set; }
        public string? ModuleName { get; set; }
        public string? AgentCode { get; set; }
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileUrl { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}