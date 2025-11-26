namespace ASOFT.CoreAI.Entities
{
    public class RawContent
    {
        public string? Text { get; init; }

        public ReadOnlyMemory<byte>? Image { get; init; }

        public int PageNumber { get; init; }
    }
}