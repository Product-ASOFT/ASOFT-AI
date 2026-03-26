using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IOOT9002Queries
    {
        Task<bool> SaveData(OOT9002 data, CancellationToken cancellationToken = default);
    }
}
