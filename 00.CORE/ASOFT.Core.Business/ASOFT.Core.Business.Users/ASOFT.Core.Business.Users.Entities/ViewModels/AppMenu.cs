using System.Collections.Generic;

namespace ASOFT.Core.Business.Users.Entities.ViewModels
{
    public class AppMenu
    {
        public string MenuAppID { get; set; }
        public string MenuText { get; set; }
        public int sysMenuID { get; set; }
        public int MenuLevel { get; set; }
        public string ModuleID { get; set; }
        public int sysMenuParent { get; set; }
        public int MenuOrder { get; set; }
        public byte? IsView { get; set; }
        public byte? IsHidden { get; set; }
        public List<AppMenu> Children { get; set; }
    }
}
