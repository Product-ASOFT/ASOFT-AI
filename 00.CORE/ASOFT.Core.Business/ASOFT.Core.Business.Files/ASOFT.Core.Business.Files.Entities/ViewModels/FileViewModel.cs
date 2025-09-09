// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using Newtonsoft.Json;
using System;

namespace ASOFT.Core.Business.Files.Entities.ViewModels
{
    /// <summary>
    /// View model cho file
    /// </summary>
    public class FileViewModel
    {
        [JsonIgnore]
        public int? TotalRow { get; set; }
        public Guid APK { get; set; }
        public string DivisionID { get; set; }
        public string AttachID { get; set; }
        public string AttachName { get; set; }
        public string CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
    }
}
