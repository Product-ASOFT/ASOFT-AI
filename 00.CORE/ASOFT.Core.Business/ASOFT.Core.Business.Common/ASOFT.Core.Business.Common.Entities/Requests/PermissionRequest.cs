namespace ASOFT.Core.Business.Common.Requests
{
    public class PermissionRequest
    {
        public virtual string DivisionID { get; set; }
        public virtual string UserID { get; set; }
        public string ModuleID { get; set; }
        public string DataID { get; set; }
        public string DataType { get; set; }
        public string GroupID { get; set; }
        public byte IsPrint { get; set; }
        public int Permission { get; set; }
        public string Condition { get; set; }
    }
}