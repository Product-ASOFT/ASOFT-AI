using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities.ViewModels.System
{
    public class LLMViewModel
    {
        public string BaseUrl { get; set; }
        public int MaxToken { get; set; }
        public double Temperature { get; set; }
        public bool IsUse {  get; set; }
    }
}
