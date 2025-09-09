
using ASOFT.Core.Common.InjectionChecker;

namespace ASOFT.Core.Business.Common.ViewModels
{
    /// <summary>
    /// Thông tin để tạo số chứng từ.
    /// </summary>
    public class VoucherInfo
    {
        /// <summary>
        /// Đơn vị
        /// </summary>
        public string DivisionID { get; }

        /// <summary>
        /// Tên bảng
        /// </summary>
        public string TableID { get; }

        /// <summary>
        /// Tháng
        /// </summary>
        public int TranMonth { get; }

        /// <summary>
        /// Năm
        /// </summary>
        public int TranYear { get; }

        /// <summary>
        /// Loại chứng từ
        /// </summary>
        public string VoucherTypeID { get; }

        /// <summary>
        /// Thông tin cho việc tạo số chừng từ.
        /// </summary>
        /// <param name="divisionId"></param>
        /// <param name="tableId"></param>
        /// <param name="tranMonth"></param>
        /// <param name="tranYear"></param>
        /// <param name="voucherTypeId"></param>
        public VoucherInfo(string divisionId, string tableId, int tranMonth, int tranYear, string voucherTypeId)
        {
            DivisionID = Checker.NotEmpty(divisionId, nameof(divisionId));
            TableID = Checker.NotEmpty(tableId, nameof(tableId));
            TranMonth = tranMonth;
            TranYear = tranYear;
            VoucherTypeID = Checker.NotEmpty(voucherTypeId, nameof(voucherTypeId));
        }
    }
}