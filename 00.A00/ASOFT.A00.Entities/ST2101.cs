// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved. 
// #
// # History:
// #	Date Time	    Updated		    Content
// #    25/01/2021      Tấn Thành       Tạo mới
// #################################################################

using System;

namespace ASOFT.A00.Entities
{
    public class ST2101
    {
        public string COL_APK = "APK";
        public string COL_DivisionID = "DivisionID";
        public string COL_GroupID = "GroupID";
        public string COL_KeyName = "KeyName";
        public string COL_KeyValue = "KeyValue";
        public string COL_ValueDataType = "ValueDataType";
        public string COL_DefaultValue = "DefaultValue";
        public string COL_ModuleID = "ModuleID";
        public string COL_IsEnvironment = "IsEnviroment";
        public string COL_Description = "Description";
        public string COL_CreateUserID = "CreateUserID";
        public string COL_CreateDate = "CreateDate";
        public string COL_LastModifyUserID = "LastModifyUserID";
        public string COL_LastModifyDate = "LastModifyDate";

        public string APK { get; set; }
        public string DivisionID { get; set; }
        public int GroupID { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public int ValueDataType { get; set; }
        public int DefaultValue { get; set; }
        public string ModuleID { get; set; }
        public int IsEnvironment { get; set; }
        public string Description { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastModifyUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
