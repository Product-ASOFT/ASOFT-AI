// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess.Entities;

namespace ASOFT.Core.Business.Common.Entities
{
    public class CRMT90031 : BusinessEntity
    {
        /// <summary>
        /// ID của ghi chú
        /// </summary>
        public int NotesID { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string NotesSubject { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Description { get; set; }
    }
}
