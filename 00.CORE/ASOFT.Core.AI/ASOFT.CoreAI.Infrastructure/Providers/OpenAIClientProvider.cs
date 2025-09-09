using OpenAI;

namespace ASOFT.CoreAI.Infrastructure
{
    public class OpenAIClientProvider : IOpenAIClientProvider, IDisposable
    {
        private OpenAIClient? _client;
        private string? _lastApiKey;
        private readonly IAIConfigStore _configStore;

        public OpenAIClientProvider(IAIConfigStore configStore)
        {
            _configStore = configStore;
            _configStore.ConfigChanged += ReloadClient;
            ReloadClient();
        }

        private void ReloadClient()
        {
            var apiKey = _configStore.GetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _client = null;
                _lastApiKey = null;
                Console.WriteLine("[OpenAIClientProvider] API key null/empty, client cleared.");
            }
            else if (_lastApiKey != apiKey)
            {
                _client = new OpenAIClient(apiKey);
                _lastApiKey = apiKey;
                Console.WriteLine("[OpenAIClientProvider] Reloaded OpenAIClient with new API key.");
            }
        }

        public OpenAIClient? GetClient() => _client;

        public void Dispose()
        {
            _configStore.ConfigChanged -= ReloadClient;
        }
    }
}