namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    public class ScreenPermission
    {
        public string UserID { get; set; }
        public string ModuleID { get; set; }
        public string ScreenID { get; set; }
        public string DivisionID { get; set; }
        public byte? IsAddNew { get; set; }
        public byte? IsUpdate { get; set; }
        public byte? IsDelete { get; set; }
        public byte? IsView { get; set; }
        public byte? IsPrint { get; set; }
        public byte? IsExportExcel { get; set; }
    }
}
