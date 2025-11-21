using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities.ViewModels.AI
{

    public class CriteriaSummaryResult
    {
        public List<ST2136> Criteria { get; set; }
        public string OverallResult { get; set; }
        public string OverallReason { get; set; }
    }
}
