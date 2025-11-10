namespace ASOFT.CoreAI.Entities.ViewModels.System
{
    public class ChatResponseModel
    {
        public Guid? ChatSessionID { get; set; } = Guid.Empty;
        public string Result { get; set; }
        public string? StatusCode { get; set; }
        public bool? Status { get; set; }
    }

    public class ChatResponseReadFileModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ST2131>? ReadFileResults { get; set; } = null;
    }
}