namespace ASOFT.CoreAI.Entities
{
    public class ChatbotConfig
    {
        public string APK { get; set; }
        public string ChatbotName { get; set; }
        public string UrlAPI { get; set; }
        public string APIKey { get; set; }
        public int? MaxToken { get; set; }
        public bool IsSendImage { get; set; }
        public bool IsUse { get; set; }
        public string ChatBotAPIType { get; set; } // S1.Description
        public string ChatbotModel { get; set; }   // S2.Description
    }
}