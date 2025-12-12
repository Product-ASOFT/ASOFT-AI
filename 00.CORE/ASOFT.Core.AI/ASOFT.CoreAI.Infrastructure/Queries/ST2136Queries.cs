using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public class ST2136Queries : IST2136Queries
    {

        private readonly IBusinessContext<ST2136> _businessContext;

        public ST2136Queries(IBusinessContext<ST2136> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }

        public async Task<bool> DeleteData(IEnumerable<ST2136> datas, CancellationToken cancellationToken = default)
        {
            return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async tran =>
            {
                await _businessContext.BulkDeleteAsync(datas);
                await _businessContext.UnitOfWork.CompleteAsync();
                return true;
            });
        }

        public async Task<List<ST2136>> GetData(string BusinessParent, CancellationToken cancellationToken = default)
        {
            return await _businessContext.QueryAsync(new FilterQuery<ST2136>(m => m.BusinessParent == BusinessParent));
        }

        public Task SaveData(IEnumerable<ST2136> datas, CancellationToken cancellationToken = default)
        {
            return _businessContext.UnitOfWork.ExecuteInTransactionAsync(
            async transactionHolder =>
            {
                await _businessContext.AddRangeAsync(datas, cancellationToken);
                await _businessContext.UnitOfWork.CompleteAsync();
            });
        }
    }
}
