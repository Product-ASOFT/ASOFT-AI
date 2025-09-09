using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business.Functions;

public sealed class PromptRenderingResult
{
    public IAIService AIService { get; set; }

    public string RenderedPrompt { get; set; }

    public PromptExecutionSettings? ExecutionSettings { get; set; }

    public FunctionResult? FunctionResult { get; set; }

    public PromptRenderingResult(IAIService aiService, string renderedPrompt)
    {
        this.AIService = aiService;
        this.RenderedPrompt = renderedPrompt;
    }
}