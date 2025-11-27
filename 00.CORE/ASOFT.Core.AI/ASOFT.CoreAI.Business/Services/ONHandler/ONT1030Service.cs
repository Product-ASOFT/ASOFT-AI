using ASOFT.Core.DataAccess;

// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.
// #
// # History：
// #	Date Time	    Updated		    Content
// #    26/11/2025      Đức Mạnh        Tạo mới
// ##################################################################

using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class ONT1030Service : IONT1030Service
    {
        private readonly IBusinessContext<ONT1030> _businessContext;

        public ONT1030Service(IBusinessContext<ONT1030> businessContext)
        {
            _businessContext = businessContext;
        }

        public Task<ONT1030> GetAIModelAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ONT1030>> GetAIModelsAsync()
        {
            return await _businessContext.QueryAsync(new FilterQuery<ONT1030>(m => m.IsUse == 1));
        }
    }
}