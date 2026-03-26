using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IOOT9003Queries
    {
        Task<bool> SaveData(IEnumerable<OOT9003> data, CancellationToken cancellationToken = default);
    }
}
