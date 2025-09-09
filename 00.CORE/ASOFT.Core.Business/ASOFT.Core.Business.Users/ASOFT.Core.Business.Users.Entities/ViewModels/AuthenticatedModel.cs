using ASOFT.A00.Entities.ViewModels;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using System.Collections.Generic;

namespace ASOFT.Core.Business.Users.Entities
{
    public class AuthenticatedModel
    {
        public string DivisionID { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string DivisionName { get; set; }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public int? PageSize { get; set; }
        public string LanguageID { get; set; }

        /// <summary>
        /// Nhóm
        /// </summary>
        public string GroupID { get; set; }

        public string UserJoinRoleID { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        public IEnumerable<DivisionModel> DivisionIDList { get; set; }

        /// <summary>
        /// Dữ liệu phân quyền màn hình
        /// </summary>
        public IEnumerable<AP1403ViewModel> ScreenPermissions { get; set; }
        public Dictionary<string, AppMenu> Menu { get; set; }

        //Dữ liệu cho user đăng nhập là khách hàng
        public string DeliveryAddress { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public byte? IsCustomer { get; set; }
    }
}