namespace ASOFT.CoreAI.Entities
{
    public class BEMT2001Model
    {
        public int RowNum { get; set; }
        public int TotalRow { get; set; }
        public string APK { get; set; }
        public string? APKMaster { get; set; }
        public string? APKMaster_9000 { get; set; }
        public string? DivisionID { get; set; }
        public string? APKMInherited { get; set; }
        public string? APKDInherited { get; set; }
        public string? InheritVoucherNo { get; set; }
        public string? InheritType { get; set; }
        public string? DepartmentAnaID { get; set; }
        public string? DepartmentAnaName { get; set; }
        public string? CostAnaID { get; set; }
        public string? CostAnaName { get; set; }
        public string? Description { get; set; }
        public string? RingiNo { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? FeeID { get; set; }
        public string? FeeName { get; set; }
        public string? CurrencyID { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? RequestAmount { get; set; }
        public decimal? ConvertedGeneralAmount { get; set; }
        public decimal? ConvertedRequestAmount { get; set; }
        public decimal? SpendAmount { get; set; }
        public decimal? ConvertedSpendAmount { get; set; }
        public string? BankAccountID { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankAccountName { get; set; }
        public int ApprovingLevel { get; set; }
        public int ApproveLevel { get; set; }
        public string? DebitAccountID { get; set; }
        public string? CreditAccountID { get; set; }
        public string? MediumAccountID { get; set; }
        public string? DebitAcccountGroupID { get; set; }
        public string? ListAPKDInherited { get; set; }
        public int DeleteFlg { get; set; }
        public int OrderNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateUserID { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string? LastModifyUserID { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int Status { get; set; }
        public string? TVoucherID { get; set; }
        public string? TBatchID { get; set; }
        public string? APK900101 { get; set; }
        public int ApprovePerson01Status { get; set; }
        public string? ApprovePerson01StatusName { get; set; }
        public string? ApprovePerson01Note { get; set; }
        public DateTime? ApprovePerson01Date { get; set; }
        public string? APK900102 { get; set; }
        public int ApprovePerson02Status { get; set; }
        public string? ApprovePerson02StatusName { get; set; }
        public string? ApprovePerson02Note { get; set; }
        public DateTime? ApprovePerson02Date { get; set; }

        public string InvoiceDateFormatted
        {
            get
            {
                if (this.InvoiceDate.HasValue)
                {
                    return this.InvoiceDate.Value.ToString("dd/MM/yyyy");
                }
                return string.Empty;
            }
        }
    }
}