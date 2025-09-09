using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Infrastructure;
using System.Text.Json;

public class AIConfigStore : IAIConfigStore
{
    private string? _apiKey;
    private string? _modelName;
    private readonly object _lock = new();

    private readonly IRedisMemoryProvider _vectorDatabase;

    public event Action? ConfigChanged;

    public AIConfigStore(IRedisMemoryProvider vectorDatabase)
    {
        _vectorDatabase = vectorDatabase;
    }

    public string? GetApiKey()
    {
        lock (_lock) return _apiKey;
    }

    public string? GetModelName()
    {
        lock (_lock) return _modelName;
    }

    public void SetApiKey(string? apiKey)
    {
        bool changed = false;
        lock (_lock)
        {
            if (_apiKey != apiKey)
            {
                _apiKey = apiKey;
                changed = true;
            }
        }
        if (changed) ConfigChanged?.Invoke();
    }

    public void SetModelName(string? modelName)
    {
        bool changed = false;
        lock (_lock)
        {
            if (_modelName != modelName)
            {
                _modelName = modelName;
                changed = true;
            }
        }
        if (changed) ConfigChanged?.Invoke();
    }

    public async Task LoadConfigFromRedisAsync()
    {
        var config = await _vectorDatabase.GetOpenAIChatConfigAsync(AIConstants.ModelAIKey);
        if (config == null)
        {
            // Nếu không có config thì xóa hết dữ liệu cũ
            SetApiKey(null);
            SetModelName(null);
            return;
        }
        try
        {
            SetApiKey(config?.ApiKey);
            SetModelName(config?.ModelName);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Lỗi deserialize JSON từ Redis: {ex.Message}");
            SetApiKey(null);
            SetModelName(null);
        }
    }
}