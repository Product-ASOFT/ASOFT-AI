using ASOFT.Core.DataAccess.Entities;

namespace ASOFT.CoreAI.Entities
{
    public class ST2131 : BaseEntity
    {
        public Guid? APKMaster { get; set; }
        public int AttachID { get; set; }
        public string? AttachName { get; set; }
        public string? Status { get; set; }
        public string? Percentage { get; set; }
        public string? TextContentOCR { get; set; }
        public string? TextContentAI { get; set; }
        public string? Note { get; set; }
        public string? StatusProcess { get; set; }
    }
}