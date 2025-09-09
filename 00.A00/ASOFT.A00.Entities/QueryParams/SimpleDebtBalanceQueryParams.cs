using System;

namespace ASOFT.A00.Entities.QueryParams
{
    public class SimpleDebtBalanceQueryParams
    {
        public string DivisionID { get; set; }
        public string ObjectID { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string CurrencyID { get; set; }
        public string Type { get; set; }
    }
}