// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess.Entities;

namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Bảng CRMT0002
    /// </summary>
    public class CRMT00002 : BaseEntity
    {
        public int? AttachID { get; set; }
        public byte[] AttachFile { get; set; }
        public string AttachName { get; set; }
    }
}
