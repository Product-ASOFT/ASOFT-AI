// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################


using System.Collections.Generic;

namespace ASOFT.Core.Business.Files.Entities.Respones
{
    public class UploadRespone
    {
        /// <summary>
        /// Số file upload thành công
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Tổng dung lượng
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Danh sách chi tiết kết quả thành công thất bại cho từng file
        /// </summary>
        public List<FileUploadResult> ResultList { get; set; }

    }
}
