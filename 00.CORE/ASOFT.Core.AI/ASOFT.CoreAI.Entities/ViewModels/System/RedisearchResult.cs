namespace ASOFT.CoreAI.Entities
{
    public class RedisearchResultItem
    {
        public string Id { get; set; }
        public FieldsData FieldDatas { get; set; }
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
        public string Text => FieldDatas?.Text;
        public string ReferenceDescription => FieldDatas?.ReferenceDescription;
        public string ReferenceLink => FieldDatas?.ReferenceLink;
    }

    public class FieldsData
    {
        public string Text { get; set; }
        public string ReferenceDescription { get; set; }
        public string ReferenceLink { get; set; }
    }
}