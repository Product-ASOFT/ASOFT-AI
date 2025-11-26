namespace ASOFT.CoreAI.Entities
{
    public class CustomMemoryRecord
    {
        public string Key { get; set; } = Guid.NewGuid().ToString();
        public string Data { get; set; }
        public string Embedding { get; set; }
        public string Prompt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
        public string? Module { get; set; }
        public string Text { get; set; }
        public string CollectionName { get; set; }
        public string AgentCode { get; set; }
    }
}