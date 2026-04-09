using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IBEMT2004Queries
    {
        Task<List<BEMT2004>> GetData(string BusinessParent, CancellationToken cancellationToken = default);
        Task SaveData(IEnumerable<BEMT2004> datas, CancellationToken cancellationToken = default);
        Task<bool> DeleteData(Guid APKMaster, CancellationToken cancellationToken = default);
    }
}
