// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

namespace ASOFT.Core.Business.Notification.Entities
{
    /// <summary>
    /// Const dùng cho notification
    /// </summary>
    public class DataMessageCommandType
    {
        /// <summary>
        /// Load lại số thông báo đã đọc
        /// </summary>
        public static readonly string RELOAD_NOTIFICATION_COUNT = "RELOAD_NOTIFICATION_COUNT";

        /// <summary>
        /// Logout APP
        /// </summary>
        public static readonly string LOGOUT  = "LOGOUT";
    }
}