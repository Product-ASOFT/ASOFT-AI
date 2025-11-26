using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class ONT1030ViewModel: BaseEntity
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public required string ModelName { get; set; }
        public required string APIKey { get; set; }
        public string UrlAPI { get; set; }
        public string ModelEmbedding { get; set; }
        public string Teamperature { get; set; }
        public string MaxToken { get; set; }
        public bool IsUse { get; set; }
    }
}
