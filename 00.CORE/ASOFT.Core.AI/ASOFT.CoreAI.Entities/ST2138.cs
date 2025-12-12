using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class ST2138 : BaseEntity
    {
        public Guid APKMaster_ST2137 { get; set; }
        public int OrderNo { get; set; }
        public string? VoucherNo { get; set; }
        public string? VoucherName { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string? FileName { get; set; }
        public string? PaymentTerm { get; set; }
        public string? DeliveryTerm { get; set; }
        public string? ClearanceStatus { get; set; }
        public DateTime? ClearanceDate { get; set; }
        public string? RingiNo { get; set; }
        public string? ContractNo { get; set; }
        public string? GoodsName { get; set; }
        public decimal Quantity { get; set; }
        public string? ExtraJson { get; set; }

    }
}
