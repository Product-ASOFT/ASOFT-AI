namespace ASOFT.CoreAI.Entities
{
    public class ChatHistoryResponseModel
    {
        public Guid ChatSessionID { get; set; }
        public string UserID { get; set; }
        public string Message { get; set; }
        public string ResponseText { get; set; }
        public DateTime CreateDate { get; set; }
    }
}