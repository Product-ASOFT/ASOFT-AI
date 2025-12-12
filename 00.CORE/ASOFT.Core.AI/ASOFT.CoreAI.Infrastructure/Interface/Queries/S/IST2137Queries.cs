using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2137Queries
    {
        Task SaveData(IEnumerable<ST2137> results, CancellationToken cancellationToken = default);
        Task<bool> DeleteData(IEnumerable<ST2137> results, CancellationToken cancellationToken = default);
    }
}
