using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.Core.Business.Common.Entities
{
    public class OOT9002
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid APK { get; set; }

        public string CreateUserID { get; set; }

        public DateTime? CreateDate { get; set; }

        public string LastModifyUserID { get; set; }

        public DateTime? LastModifyDate { get; set; }

        public Guid? APKMaster { get; set; }

        public string Description { get; set; }
        public int? ScreenType { get; set; }
        public int? ShowType { get; set; }
        public int? BusinessTypeID { get; set; }
        //public int MessageType { get; set; }
        public string ScreenID { get; set; }
        public string ScreenName { get; set; }
        public string ModuleID { get; set; }
        public string UrlCustom { get; set; }
        public string Parameters { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public byte? DeleteFlag { get; set; }
        public byte? Disabled { get; set; }
        public DateTime? EffectDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // [Tấn Thành] - [15/01/2021] - Begin add
        public const string COL_APKMaster = "APKMaster";
        public const string COL_APK = "APK";
        public const string COL_Description = "Description";
        public const string COL_ScreenType = "ScreenType";
        public const string COL_ModuleID = "ModuleID";
        public const string COL_ScreenID = "ScreenID";
        public const string COL_ScreenName = "ScreenName";
        public const string COL_Parameters = "Parameters";
        public const string COL_UrlCustom = "URlCustom";
        public const string COL_DeleteFlag = "DeleteFlag";
        public const string COL_CreateDate = "CreateDate";
        public const string COL_CreateUserID = "CreateUserID";
        public const string COL_LastModifyDate = "LastModifyDate";
        public const string COL_LastModifyUserID = "LastModifyUserID";
        public const string COL_Title = "Title";
        public const string COL_ShowType = "ShowType";
        public const string COL_EffectDate = "EffectDate";
        public const string COL_ExpiryDate = "ExpiryDate";
        public const string COL_Disabled = "Disabled";
        public const string COL_BusinessTypeID = "BusinessTypeID";
        // [Tấn Thành] - [15/01/2021] - End add
    }
}
