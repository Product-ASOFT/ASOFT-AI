using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IONT1021Service
    {
        Task<IEnumerable<ONT1021ViewModel>> GetAllAsync(List<int> CategoryIDs);
    }
}
