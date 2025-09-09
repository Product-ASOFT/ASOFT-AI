namespace ASOFT.CoreAI.Infrastructure
{
    public interface IAIService
    {
        /// <summary>
        /// Gets the AI service attributes.
        /// </summary>
        IReadOnlyDictionary<string, object?> Attributes { get; }
    }
}