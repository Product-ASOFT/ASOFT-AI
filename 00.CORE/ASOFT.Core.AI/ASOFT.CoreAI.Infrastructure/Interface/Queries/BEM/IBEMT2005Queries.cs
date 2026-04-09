using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IBEMT2005Queries
    {
        Task SaveData(IEnumerable<BEMT2005> results, CancellationToken cancellationToken = default);
        Task<bool> DeleteData(Guid apkMater_BEMT2003, CancellationToken cancellationToken = default);
    }
}
