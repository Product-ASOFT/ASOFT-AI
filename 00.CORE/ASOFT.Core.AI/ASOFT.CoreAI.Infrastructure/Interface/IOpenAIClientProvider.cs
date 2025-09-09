using OpenAI;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IOpenAIClientProvider
    {
        OpenAIClient? GetClient();
    }
}