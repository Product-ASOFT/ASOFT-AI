using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.Interface
{
    public interface IReadFileBackgroundWorkflow
    {
        Task RunAsync(Guid BEMT2003APK, ReadFileRequest request, List<PromptContentViewModel> promptContents, CancellationToken ct = default);
    }
}
