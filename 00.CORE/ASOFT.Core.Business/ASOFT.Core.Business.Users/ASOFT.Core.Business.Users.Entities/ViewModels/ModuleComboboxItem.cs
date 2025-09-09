using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Entities.ViewModels
{
    public class ModuleComboboxItem
    {
        #region ---- Enums, Structs, Constants ----

        public const string COL_MODULEID = "ModuleID";
        public const string COL_MODULENAME = "ModuleName";

        #endregion ---- Enums, Structs, Constants ----

        #region ---- Properties ----

        public string ModuleID { set; get; }
        public string ModuleName { set; get; }

        #endregion ---- Properties ----
    }
}
