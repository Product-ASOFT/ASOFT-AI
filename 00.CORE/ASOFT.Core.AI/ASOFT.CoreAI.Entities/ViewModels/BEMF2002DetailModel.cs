namespace ASOFT.CoreAI.Entities
{
    public class BEMF2002DetailModel
    {
        public Guid APK { get; set; }
        public string? DivisionID { get; set; }
        public Guid APKMaster_9000 { get; set; }
        public string? Levels { get; set; }
        public int TranMonth { get; set; }
        public int TranYear { get; set; }
        public string? VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string? TypeID { get; set; }
        public string? TypeName { get; set; }
        public string? DepartmentID { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ApplicantID { get; set; }
        public string? ApplicantName { get; set; }
        public string? MethodPay { get; set; }
        public string? PaymentTermID { get; set; }
        public string? FCT { get; set; }
        public decimal? AdvancePayment { get; set; }
        public string? AdvanceUserID { get; set; }
        public string? AdvanceUserName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Status { get; set; }
        public int DeleteFlg { get; set; }
        public Guid? APKInherited { get; set; }
        public string? InheritVoucherNo { get; set; }
        public string? DescriptionMaster { get; set; }
        public string? ApprovingLevel { get; set; }
        public int ApproveLevel { get; set; }
        public string? CurrencyID { get; set; }
        public decimal ExchangeRate { get; set; }
        public string? CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? BankAccountNo { get; set; }
        public string? LastModifyUserID { get; set; }
        public DateTime LastModifyDate { get; set; }

        // Người duyệt cấp 1
        public Guid? APK900101 { get; set; }

        public string? ApprovePerson01Name { get; set; }
        public string? ApprovePerson01Status { get; set; }
        public string? ApprovePerson01Note { get; set; }

        // Người duyệt cấp 2
        public Guid? APK900102 { get; set; }

        public string? ApprovePerson02Name { get; set; }
        public string? ApprovePerson02Status { get; set; }
        public string? ApprovePerson02Note { get; set; }

        // Format
        public string VoucherDateFormatted
        {
            get
            {
                if (this.VoucherDate.HasValue)
                {
                    return this.VoucherDate.Value.ToString("dd/MM/yyyy");
                }
                return string.Empty;
            }
        }

        public string DeadlineFormatted
        {
            get
            {
                if (this.Deadline.HasValue)
                {
                    return this.Deadline.Value.ToString("dd/MM/yyyy");
                }
                return string.Empty;
            }
        }

        public string AdvancePaymentFormatted
        {
            get
            {
                if (this.AdvancePayment.HasValue)
                {
                    return this.AdvancePayment.Value.ToString("N0");
                }
                return string.Empty;
            }
        }

        public string CreateDateFormatted
        {
            get
            {
                if (this.CreateDate.HasValue)
                {
                    return this.CreateDate.Value.ToString("dd/MM/yyyy");
                }
                return string.Empty;
            }
        }
    }
}