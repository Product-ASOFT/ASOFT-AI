namespace ASOFT.CoreAI.Infrastructure
{
    public interface IAIConfigStore
    {
        event Action? ConfigChanged;

        string? GetApiKey();

        string? GetModelName();

        void SetApiKey(string? apiKey);

        void SetModelName(string? modelName);
    }
}