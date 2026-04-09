using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.Queries
{
    public class ONT1041Queries : IONT1041Queries
    {

        private readonly IBusinessContext<ONT1041> _businessContext;

        public ONT1041Queries(IBusinessContext<ONT1041> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }

        public async Task<IEnumerable<ONT1041>> GetAllByParameterRoleAsync(int parameterRole)
        {
            return await _businessContext.QueryAsync(new FilterQuery<ONT1041>(m => m.ParameterRole == parameterRole));
        }
    }
}
