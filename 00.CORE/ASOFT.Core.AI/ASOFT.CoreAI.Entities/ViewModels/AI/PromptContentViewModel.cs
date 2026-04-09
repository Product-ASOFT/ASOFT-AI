using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities.ViewModels.AI
{
    public class PromptContentViewModel
    {
        public Guid APK { get; set; }
        public string CriteriaName { get; set; } = null!;
        public string PromptUser { get; set; } = null!;
        public string PromptSystem { get; set; } = null!;
    }
}
