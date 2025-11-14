using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.Interface
{
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(ReadFileJob job, CancellationToken ct = default);
        ValueTask<ReadFileJob> DequeueAsync(CancellationToken ct);
    }
}
