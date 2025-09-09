using System;

namespace ASOFT.Core.Business.Users.Entities
{
    public class AuthenticationUserInfo
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
        public string Password { get; set; }

        /// <summary>
        /// Nhóm
        /// </summary>
        public string GroupID { get; set; }

        public string UserJoinRoleID { get; set; }
        public string DeliveryAddress { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public byte? IsCustomer { get; set; }
        public string BiometricsKey {  get; set; }
        public byte? Disabled { set; get; }
        public string CreateUserID { set; get; }
        public DateTime? CreateDate { set; get; }
        public string LastModifyUserID { set; get; }
        public DateTime? LastModifyDate { set; get; }
        public byte? IsLock { set; get; }
        public string UserToken { set; get; }
        public DateTime? TimeExpiredToken { set; get; }
        public string DepartmentID { set; get; }
    }
}