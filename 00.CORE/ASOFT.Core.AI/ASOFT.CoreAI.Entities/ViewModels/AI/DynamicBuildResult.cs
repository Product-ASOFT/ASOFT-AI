namespace ASOFT.CoreAI.Entities
{
    public class BuildDetailResult
    {
        public List<BEMT2006> Entities { get; set; } = new();
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
    }
}