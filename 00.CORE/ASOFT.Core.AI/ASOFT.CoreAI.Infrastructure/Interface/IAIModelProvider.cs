namespace ASOFT.CoreAI.Infrastructure
{
    public interface IAIModelKeyProvider
    {
        Task<string> GetApiKeyAsync(string modelName);

        Task InitializeAllAsync(); // gọi khi startup lấy tất cả key cache sẵn
    }
}