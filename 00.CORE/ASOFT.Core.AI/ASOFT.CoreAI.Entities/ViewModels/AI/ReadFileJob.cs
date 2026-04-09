using ASOFT.CoreAI.Entities.ViewModels.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public sealed record ReadFileJob(Guid BEMT2003APK, ReadFileRequest request, List<PromptContentViewModel> PromptContents);
}
