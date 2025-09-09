using System.ComponentModel.DataAnnotations;

namespace ASOFT.A00.Entities.QueryParams
{
    public class CalculatingUnitOfConversionQueryParams
    {
        public decimal? T01 { get; set; }
        public decimal? T02 { get; set; }
        public decimal? T03 { get; set; }
        public decimal? T04 { get; set; }
        public decimal? T05 { get; set; }
        public decimal? SL { get; set; }

        [StringLength(50)] public string FormulaDes { get; set; }
    }
}