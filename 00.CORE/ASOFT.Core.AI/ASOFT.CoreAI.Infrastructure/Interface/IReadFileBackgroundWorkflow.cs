using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.Interface
{
    public interface IReadFileBackgroundWorkflow
    {
        Task RunAsync(Guid ST2131APK, ReadFileRequest request, string promptSystem, string promptContent, CancellationToken ct = default);
    }
}
