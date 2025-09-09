// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ASOFT.Core.Business.Files.Entities.Requests
{
    public class UploadFileRequest
    {
        public List<IFormFile> Files { get; set; }
        public string DivisionID { get; set; }
        public string UserID { get; set; }
        public string TableID { get; set; }
        public Guid APK { get; set; }
    }
}
