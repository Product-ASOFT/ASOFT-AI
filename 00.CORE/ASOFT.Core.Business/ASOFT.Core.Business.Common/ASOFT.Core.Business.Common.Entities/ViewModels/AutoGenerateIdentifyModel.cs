namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    /// <summary>
    /// Model thiết lập cho số chứng từ.
    /// </summary>
    public class AutoGenerateIdentifyModel
    {
        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public virtual string DivisionID { get; set; }

        /// <summary>
        /// Mã cửa hàng
        /// </summary>
        public virtual string ShopID { get; set; }

        /// <summary>
        /// Tên bảng danh mục hoặc tên bảng nghiệp vụ
        /// </summary>
        public virtual string TableName { get; set; }

        /// <summary>
        /// Xác định việc cho phép tăng mã tự động
        /// </summary>
        public virtual int IsAutomatic { get; set; }

        /// <summary>
        /// Thứ tự xuất ra phần của mã
        /// </summary>
        public virtual int OutputOrder { get; set; }

        /// <summary>
        /// Xác định cho phép phần S1 của mã
        /// </summary>
        public virtual int Enable1 { get; set; }

        /// <summary>
        /// Xác định cho phép phần S2 của mã
        /// </summary>
        public virtual int Enable2 { get; set; }

        /// <summary>
        /// Xác định cho phép phần S3 của mã
        /// </summary>
        public virtual int Enable3 { get; set; }

        /// <summary>
        /// Chuổi khóa - mẫu của mã
        /// </summary>
        public virtual string KeyString { get; set; }

        /// <summary>
        /// Ký tự (hoặc chuỗi) phân cách
        /// </summary>
        public virtual string Separator { get; set; }

        /// <summary>
        /// Xác định cho phép hiển thị dấu phân cách
        /// </summary>
        public virtual int IsSeparated { get; set; }

        /// <summary>
        /// Phần thứ nhất của mã
        /// </summary>
        public virtual string S1 { get; set; }

        /// <summary>
        /// Phần thứ hai của mã
        /// </summary>
        public virtual string S2 { get; set; }

        /// <summary>
        /// Phần thứ ba của mã
        /// </summary>
        public virtual string S3 { get; set; }

        public virtual byte? S1Type { set; get; }
        public virtual byte? S2Type { set; get; }
        public virtual byte? S3Type { set; get; }

        /// <summary>
        /// Tổng độ dài của mã (kể cả phần số thứ tự, và ký tự phân cách)
        /// </summary>
        public virtual int Length { get; set; }

        /// <summary>
        /// Số thứ tự cuối cùng 
        /// </summary>
        public virtual int LastKey { get; set; }
    }
}