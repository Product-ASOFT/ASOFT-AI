using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public sealed class ParsedCriteriaResult
    {
        public string CriteriaName { get; set; } = string.Empty;
        public string CriteriaStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public static ParsedCriteriaResult Fail(string criteriaName, string criteriaStatus, string description)
        {
            return new ParsedCriteriaResult
            {
                CriteriaName = criteriaName,
                CriteriaStatus = criteriaStatus,
                Description = description
            };
        }
    }
}
