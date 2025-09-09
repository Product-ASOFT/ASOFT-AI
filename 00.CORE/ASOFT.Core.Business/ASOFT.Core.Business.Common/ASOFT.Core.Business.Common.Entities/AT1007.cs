using ASOFT.Core.DataAccess.Entities;

namespace ASOFT.Core.Business.Common.Entities
{
    /// <summary>
    /// Loại chứng từ.
    /// </summary>
    public class AT1007 : CategoryEntity
    {
        public string VoucherTypeID { get; set; }
        public string VoucherTypeName { get; set; }
        public byte IsDefault { get; set; }
        public string DebitAccountID { get; set; }
        public string CreditAccountID { get; set; }
        public string ObjectID { get; set; }
        public string WareHouseID { get; set; }
        public string VDescription { get; set; }
        public string TDescription { get; set; }
        public string BDescription { get; set; }
        public byte Auto { get; set; }
        public string S1 { get; set; }
        public string S2 { get; set; }
        public string S3 { get; set; }
        public byte? OutputOrder { get; set; }
        public byte? OutputLength { get; set; }
        public byte Separated { get; set; }
        public string Separator { get; set; }
        public byte? S1Type { get; set; }
        public byte? S2Type { get; set; }
        public byte? S3Type { get; set; }
        public byte? Enabled1 { get; set; }
        public byte? Enabled2 { get; set; }
        public byte? Enabled3 { get; set; }
        public byte Enabled { get; set; }
        public string VoucherGroupID { get; set; }
        public string BankAccountID { get; set; }
        public string BankName { get; set; }
        public byte? IsVAT { get; set; }
        public string VATTypeID { get; set; }
        public byte? IsBDescription { get; set; }
        public byte? IsTDescription { get; set; }
        public byte? IsCommon { get; set; }
        public string CurrencyID { get; set; }
        public string ExWareHouseID { get; set; }
        public string ModuleID { get; set; }
        public string ScreenID { get; set; }
        // [Tấn Thành] - [17/12/2020] - BEGIN ADD
        // Move từ ERP 9 qua
        public byte? IsSpecialAutoGen { set; get; }
        // [Tấn Thành] - [17/12/2020] - END ADD
    }
}