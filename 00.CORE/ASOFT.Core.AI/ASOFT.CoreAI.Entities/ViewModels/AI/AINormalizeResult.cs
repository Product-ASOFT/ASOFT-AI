using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class AiNormalizeResult
    {
        public List<AiSection> Sections { get; set; } = new();
    }

    public class AiSection
    {
        public AiSectionMaster Master { get; set; } = new();
        public List<AiSectionDetail> Details { get; set; } = new();
    }

    public class AiSectionMaster
    {
        public string SectionType { get; set; } = null!;
        public int SectionOrder { get; set; }
        public string? SectionTitle { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? TotalCurrency { get; set; }
        public string? Signature { get; set; }
    }

    public class AiSectionDetail
    {
        public int OrderNo { get; set; }
        public string? VoucherNo { get; set; }
        public string? VoucherName { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string? FileName { get; set; }
        public string? PaymentTerm { get; set; }
        public string? DeliveryTerm { get; set; }
        public string? ClearanceStatus { get; set; }
        public DateTime? ClearanceDate { get; set; }
        public DateTime? AcceptanceDate { get; set; }
        public DateTime? HandoverDate { get; set; }
        public DateTime? PackingListDate { get; set; }
        public string? RingiNo { get; set; }
        public string? ContractNo { get; set; }
        public string? DeclarationNo { get; set; }
        public string? BillNo { get; set; }
        public string? PackingListNo { get; set; }
        public string? GoodsName { get; set; }
        public decimal? Quantity { get; set; }
        public string? ExtraJson { get; set; }
        public string? OCRRawText { get; set; }
    }
}
