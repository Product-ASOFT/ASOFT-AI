using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IBEMT2006Queries
    {
        Task SaveData(IEnumerable<BEMT2006> results, CancellationToken cancellationToken = default);
        Task<bool> DeleteData(Guid apkMater_BEMT2003, CancellationToken cancellationToken = default);
    }
}
