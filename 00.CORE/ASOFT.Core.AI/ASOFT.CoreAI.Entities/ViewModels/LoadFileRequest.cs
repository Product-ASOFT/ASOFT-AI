namespace ASOFT.CoreAI.Entities
{
    public class LoadFileRequest
    {
        public string FilePath { get; set; }
        public int BatchSize { get; set; }
        public int BetweenBatchDelayInMs { get; set; }
        public string IndexName { get; set; }

        public string Prefix
        {
            get
            {
                return $"{IndexName}:";
            }
        }
    }
}