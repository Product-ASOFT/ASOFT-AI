// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/03/2021      Đoàn Duy      Tạo mới
// ##################################################################

namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    public class HistoryModel
    {
        public const string COL_APK = "APK";
        public const string COL_DIVISIONID = "DivisionID";
        public const string COL_HISTORYID = "HistoryID";
        public const string COL_DESCRIPTION = "Description";
        public const string COL_RELATEDTOID = "RelatedToID";
        public const string COL_RELATEDTOTYPEID = "RelatedToTypeID";
        public const string COL_CREATEDATE = "CreateDate";
        public const string COL_CREATEUSERID = "CreateUserID";
        public const string COL_STATUSID = "StatusID";
        public const string COL_ScreenID = "ScreenID";
        public const string COL_TableID = "TableID";

        public System.Guid? APK { set; get; }
        public string DivisionID { set; get; }
        public int? HistoryID { set; get; }
        public string Description { set; get; }
        public string RelatedToID { set; get; }
        public int? RelatedToTypeID { set; get; }
        public System.DateTime? CreateDate { set; get; }
        public string CreateUserID { set; get; }
        public int? StatusID { set; get; }
        public string ScreenID { set; get; }
        public string TableID { set; get; }
    }
}
