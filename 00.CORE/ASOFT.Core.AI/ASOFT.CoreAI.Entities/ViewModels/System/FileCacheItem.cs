namespace ASOFT.CoreAI.Entities
{
    public class FileCacheItem
    {
        public string TextContent { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public string FileName { get; set; }
    }
}