using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IONT1041Queries
    {
        Task<IEnumerable<ONT1041>> GetAllByParameterRoleAsync(int role);
    }
}
