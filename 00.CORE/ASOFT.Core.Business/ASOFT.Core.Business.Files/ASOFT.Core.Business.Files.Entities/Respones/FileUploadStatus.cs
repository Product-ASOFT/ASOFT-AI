// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/08/2020      Đoàn Duy      Tạo mới
// ##################################################################

namespace ASOFT.Core.Business.Files.Entities.Respones
{
    /// <summary>
    /// Kết quả file upload thành công hay không
    /// </summary>
    public class FileUploadResult
    {
        /// <summary>
        /// Tên file 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File đã upload thành công hay thất bại
        /// </summary>
        public bool Success { get; set; }
    }   
}
