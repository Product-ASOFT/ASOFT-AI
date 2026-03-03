using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public sealed record ReadFileJob(Guid ST2131APK, ReadFileRequest request, string promptSystem, string promptContent);
}
