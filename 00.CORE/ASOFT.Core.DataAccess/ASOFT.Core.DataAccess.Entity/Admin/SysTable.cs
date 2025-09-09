namespace ASOFT.Core.DataAccess.Entities.Admin
{
    public class SysTable
    {
        public string TableName { get; set; }

        public string PK { get; set; }

        public string ModuleID { get; set; }

        public string ParentTable { get; set; }

        public string RefLink { get; set; }

        public string RefUrl { get; set; }

        public string RelTable { get; set; }

        public string RelColumn { get; set; }

        public string TableDelete { get; set; }

        public int? TypeREL { get; set; }

        public string RealRelColumn { get; set; }

        public int? StartRowImport { get; set; }

        public string PKDetail { get; set; }

        public string OrderBy { get; set; }
    }
}
