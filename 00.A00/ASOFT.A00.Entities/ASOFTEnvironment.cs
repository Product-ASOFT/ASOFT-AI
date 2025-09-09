// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved. 
// #
// # History:
// #	Date Time	    Updated		    Content
// #    25/01/2021      Tấn Thành       Tạo mới
// #################################################################

using System.Collections.Generic;

namespace ASOFT.A00.Entities
{
    public class ASOFTEnvironment
    {
        // Biến môi trường Lịch sử cuộc gọi
        public static Dictionary<string, string> CallCenter { get; set; }
        // Biến môi trường Automation
        public static Dictionary<string, string> Automation { get; set; }

        /// <summary>
        /// DivisionID chỉ dùng cho PipeLine, automation
        /// </summary>
        public static string DivisionID_PipeLine { get; set; }
    }
}
