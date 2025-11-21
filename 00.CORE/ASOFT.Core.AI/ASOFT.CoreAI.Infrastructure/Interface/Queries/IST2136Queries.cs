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
        Task<List<ST2136>> GetResultDetail(string BusinessParent, CancellationToken cancellationToken = default);
        Task<bool> SaveResultDetail(IEnumerable<ST2136> resultDetails, CancellationToken cancellationToken = default);
        Task<bool> DeleteResultDetail(IEnumerable<ST2136> resultDetails, CancellationToken cancellationToken = default);
    }
}
