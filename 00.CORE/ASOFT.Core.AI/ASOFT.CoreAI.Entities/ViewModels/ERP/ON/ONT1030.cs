using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class ONT1030: BaseEntity
    {
        public string? Type { get; set; } 
        public string? Name { get; set; } 
        public string ModelName { get; set; } = null!;
        public string APIKey { get; set; } = null!;
        public string? ModelEmbedding { get; set; }
        public string? UrlAPI { get; set; }
        public decimal? Temperature { get; set; }
        public int? MaxToken { get; set; }
        public int? TimeoutMs { get; set; } = 60000;
        public byte IsUse { get; set; } = 0;
        public string? Description { get; set; }
        public byte Disabled { get; set; } = 0;
   
    }
}
