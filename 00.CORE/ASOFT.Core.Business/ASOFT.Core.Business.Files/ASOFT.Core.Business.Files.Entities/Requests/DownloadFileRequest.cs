// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;

namespace ASOFT.Core.Business.Files.Entities.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class DownloadFileRequest 
    {
        /// <summary>
        /// APK của file cần download
        /// </summary>
        public Guid APK { get; set; }

        public string DivisionID { get; set; }

        public string FileName { get; set; }
    }
}
