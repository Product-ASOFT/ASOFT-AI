using ASOFT.CoreAI.Entities.ViewModels.AI;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface ICIF1640DAL
    {
        Task<ChatbotConfigViewModel> GetConfigModelAI();
    }
}