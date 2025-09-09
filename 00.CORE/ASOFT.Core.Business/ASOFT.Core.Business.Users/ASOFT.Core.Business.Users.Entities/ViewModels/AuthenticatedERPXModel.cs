using ASOFT.A00.Entities.ViewModels;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ASOFT.Core.Business.Users.Entities
{
    public class AuthenticatedERPXModel
    {
        public AuthenticationUserInfo UserInfo { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        public IEnumerable<DivisionModel> DivisionIDList { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }

        /// <summary>
        /// Dữ liệu phân quyền màn hình
        /// </summary>
        public ConcurrentDictionary<string, ASOFTPermission> ScreenPermissions { get; set; }
        public IEnumerable<ERPXMenu> Menu { get; set; }
        public IEnumerable<ModuleComboboxItem> CurListModule { get; set; }
        public string FirstPeriod { get; set; }
        public string SystemMailSettingReceives { get; set; }
        public int AmountNotification { get; set; }
    }
}