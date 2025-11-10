using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.Interface
{
    public interface IValidateData
    {
        public Task<ChatResponseReadFileModel> ValidateDataReadFileAsync(ReadFileRequest request);
    }
}
