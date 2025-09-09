// #################################################################
// # Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	Updated		    Content                
// #    08/07/2021  Văn Tài			Create New	
// #    21/12/2021  Thành Sang   	Bổ sung mã MS_00ML000191
// ##################################################################

using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.A00.Entities
{
    public static partial class ASOFTConstants
    {
        #region --- Messages ---

        public class Messages
        {
            #region --- A00 ---

            /// <summary>
            ///     Kỳ kế toán không tồn tại!
            /// </summary>
            public const string MS_00ML000089 = "00ML000089";

            /// <summary>
            ///     Kỳ kế toán {0} đã bị khóa sổ. Bạn không thể Thêm/sửa / Xóa.
            /// </summary>
            public const string MS_00ML000077 = "00ML000077";

            /// <summary>
            ///     Dữ liệu đã lưu thành công.
            /// </summary>
            public const string MS_00ML000015 = "00ML000015";

            /// <summary>
            ///     Lưu không thành công!
            /// </summary>
            public const string MS_00ML000062 = "00ML000062";

            /// <summary>
            ///     Ngày phiếu không hợp lệ.
            /// </summary>
            public const string MS_00ML000140 = "00ML000140";

            /// <summary>
            ///     {0} không tồn tại.
            /// </summary>
            public const string MS_00ML000180 = "00ML000180";

            /// <summary>
            ///     {0} đã tồn tại ở danh mục đối tượng.
            /// </summary>
            public const string MS_00ML000191 = "00ML000191";

            #endregion --- A00 ---

            #region --- SM ---

            /// <summary>
            ///     Số chứng từ tại [{0}] đã bị trùng.
            /// </summary>
            public const string MS_ASML000085 = "ASML000085";

            /// <summary>
            ///     Mã nhân viên không hợp lệ tại {0}.
            /// </summary>
            public const string MS_ASML000075 = "ASML000075";

            public static string MS_00ML000238 = "MS_00ML000238";

            #endregion --- SM ---
        }

        #endregion --- Messages ---
    }
}
