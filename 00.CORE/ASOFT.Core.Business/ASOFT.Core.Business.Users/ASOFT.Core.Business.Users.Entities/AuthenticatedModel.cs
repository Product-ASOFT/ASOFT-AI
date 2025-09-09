using ASOFT.AT.Entity;
using System.Collections.Generic;

namespace ASOFT.Authentication.Models
{
    /// <summary>
    /// Đối tượng trả về client khi được chứng thực
    /// </summary>
    public class AuthenticatedModel : AT1405
    {
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string DivisionName { get; set; }

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
    }
}