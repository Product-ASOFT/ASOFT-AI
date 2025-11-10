using ASOFT.CoreAI.Entities.ViewModels.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IOCRService
    {
        Task<(string TextMerged, List<ResultReadFileModel> Results)> ReadAsync(IReadOnlyList<AttachFileModel> files, Guid APK);
    }
}
