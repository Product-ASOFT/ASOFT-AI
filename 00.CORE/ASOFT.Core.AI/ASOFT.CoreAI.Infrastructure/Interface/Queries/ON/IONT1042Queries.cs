using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IONT1042Queries
    {
        Task<List<PromptContentViewModel>> GetDataPrompt(int caseType, string? parameterName = null, string? typeConfigID = null);
    }
}
