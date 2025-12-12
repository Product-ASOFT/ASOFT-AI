using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2136Queries
    {
        Task<List<ST2136>> GetData(string BusinessParent, CancellationToken cancellationToken = default);
        Task SaveData(IEnumerable<ST2136> datas, CancellationToken cancellationToken = default);
        Task<bool> DeleteData(IEnumerable<ST2136> datas, CancellationToken cancellationToken = default);
    }
}
