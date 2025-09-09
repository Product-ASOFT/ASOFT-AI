// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Bảng CRMT00002_REL
    /// </summary>
    public class CRMT00002_REL
    {
        public Guid? APK { get; set; }
        public string DivisionID { get; set; }
        public int? AttachID { get; set; }

        /// <summary>
        /// APK của phiếu nghiệp vụ liên quan
        /// </summary>
        public string RelatedToID { get; set; }

        /// <summary>
        /// ID lấy từ systable của nghiệp vụ
        /// </summary>
        public int? RelatedToTypeID_REL { get; set; }
    }
}
