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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Business
{
    public class ONT1030Service : IONT1030Service
    {
        private readonly IBusinessContext<ONT1030ViewModel> _businessContext;
        public ONT1030Service(IBusinessContext<ONT1030ViewModel> businessContext)
        {
            _businessContext = businessContext;
        }
        public Task<ONT1030ViewModel> GetAIModelAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ONT1030ViewModel>> GetAIModelsAsync()
        {
            return await _businessContext.QueryAsync(new FilterQuery<ONT1030ViewModel>(m => m.IsUse));
        }
    }
}
