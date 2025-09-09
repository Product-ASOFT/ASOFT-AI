using System;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.Business.Common.Entities
{
    public class OOT9003
    {
        public Guid? APK { get; set; }
        public Guid? APKMaster { get; set; }
        [Required]
        public string UserID { get; set; }
        public string DivisionID { get; set; }
        public byte? IsRead { get; set; }
        public byte? DeleteFlg { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string CreateUserID { get; set; }
        public string LastModifyUserID { get; set; }


        // [Tấn Thành] - [15/01/2021] - Begin Add
        public const string COL_APKMaster = "APKMaster";
        public const string COL_APK = "APK";
        public const string COL_UserID = "UserID";
        public const string COL_DivisionID = "DivisionID";
        public const string COL_DivisionID1 = "DivisionID1";
        public const string COL_IsRead = "IsRead";
        public const string COL_DeleteFlg = "DeleteFlg";
        public const string COL_DepartmentID = "DepartmentID";
        public const string COL_CreateUserID = "CreateUserID";
        public const string COL_CreateDate = "CreateDate";
        public const string COL_LastModifyUserID = "LastModifyUserID";
        public const string COL_LastModifyDate = "LastModifyDate";
        // [Tấn Thành] - [15/01/2021] - End Add
    }
}
