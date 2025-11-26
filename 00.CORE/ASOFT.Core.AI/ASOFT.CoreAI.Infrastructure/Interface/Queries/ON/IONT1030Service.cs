using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IONT1030Service
    {
        Task<ONT1030ViewModel> GetAIModelAsync();
        Task<IEnumerable<ONT1030ViewModel>> GetAIModelsAsync();
    }
}
