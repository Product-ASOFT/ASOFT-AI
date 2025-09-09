using System;

namespace ASOFT.A00.Entities.ViewModels
{
    public class InventoryPromotionQueryParams
    {
        public string DivisionID { get; set; }
        public string ObjectID { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string InventoryID { get; set; }
        public decimal? Quantity { get; set; }
    }
}