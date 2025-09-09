// #################################################################
// # Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.
// #
// # History：
// #    Date Time       Created         Content
// #    23/06/2020      Tấn Lộc         Create New
// ##################################################################


namespace ASOFT.A00.Entities.Enums
{
    ///<summary>
    /// Danh sách các trạng thái hệ thống được định nghĩa tại bảng AT0099 theo CodeMaster = 'SystemSatus'
    ///</summary>
    ///<history>
    /// [Tấn Lộc] created on [09/12/2020]
    ///</history>
    public enum ASOFTSystemStatus
    {
        /// <summary>
        /// Trạng thái Chưa thực hiện
        /// </summary>
        UNEXECUTED = 1,

        /// <summary>
        /// Trạng thái Đang thực hiện
        /// </summary>
        PROCESSING = 2,

        /// <summary>
        /// Trạng thái Hoàn thành
        /// </summary>
        COMPLETED = 3,

        /// <summary>
        /// Trạng thái Chờ xác nhận
        /// </summary>
        CONFIRM = 4,

        /// <summary>
        /// Trạng thái Tạm ngưng
        /// </summary>
        PENDING = 5,

        /// <summary>
        /// Trạng thái ReOpen
        /// </summary>
        REOPEN = 6,

        /// <summary>
        /// Trạng thái Hủy
        /// </summary>
        CLOSED = 7,

        #region Trạng thái hệ thống cho Email
        /// <summary>
        /// Trạng thái chưa đọc - Email hệ thống
        /// </summary>
        UNREAD = 8,

        /// <summary>
        /// Trạng thái đã đọc - Email hệ thống
        /// </summary>
        READ = 9,

        /// <summary>
        /// Trạng thái đã gửi thành công - Email hệ thống
        /// </summary>
        SentSucceeded = 10,

        /// <summary>
        /// Trạng thái đã gửi thành công - Email hệ thống
        /// </summary>
        SentFailed = 11,

        #endregion
        #region Trạng thái hệ thống cho đăng ký online

        CRMT2210_UNEXECUTE = 1,
        CRMT2210_PROCESSING = 2,
        CRMT2210_CONTRACTED = 3,
        CRMT2210_TranferLead = 4,
        CRMT2210_TranferCustomer = 5,
        CRMT2210_CLOSED = 6

        #endregion
    }
}
