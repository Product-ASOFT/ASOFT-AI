using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Entities.ViewModels
{
    public class ERPXMenu
    {
        public int sysMenuID { get; set; }

        public string MenuName { get; set; }

        public string MenuText { get; set; }

        public string sysScreenID { get; set; }

        public int MenuOrder { get; set; }

        public int sysMenuParent { get; set; }

        public int CustomerIndex { get; set; }

        public string ModuleID { get; set; }

        public int ScreenType { get; set; }

        public int MenuLevel { get; set; }

        public string ImageUrl { get; set; }
    }
}
