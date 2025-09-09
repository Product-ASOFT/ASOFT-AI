using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface ICIF1640DAL
    {
        Task<ChatbotConfig> GetConfigModelAI();
    }
}