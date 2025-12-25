using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class ST2137 : BaseEntity
    {
        public Guid APKMaster_ST2131 { get; set; }
        public string SectionType { get; set; } = null!;
        public int SectionOrder { get; set; }
        public string? SectionTitle { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? TotalCurrency { get; set; }
        public string? Signature { get; set; }

    }
}
