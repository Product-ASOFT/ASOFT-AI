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
    public class BEMT2004Queries : IBEMT2004Queries
    {

        private readonly IBusinessContext<BEMT2004> _businessContext;

        public BEMT2004Queries(IBusinessContext<BEMT2004> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }

        public async Task<bool> DeleteData(Guid APKMaster, CancellationToken cancellationToken = default)
        {
            var dataDelete = await GetDataByAPKMaster(APKMaster, cancellationToken);
            if (dataDelete == null || !dataDelete.Any())
                return true;
            return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async tran =>
            {
                await _businessContext.BulkDeleteAsync(dataDelete);
                await _businessContext.UnitOfWork.CompleteAsync();
                return true;
            });
        }
        private async Task<List<BEMT2004>> GetDataByAPKMaster(Guid APKMaster, CancellationToken cancellationToken = default)
        {
            return await _businessContext.QueryAsync(new FilterQuery<BEMT2004>(m => m.APKMaster == APKMaster));
        }
        public async Task<List<BEMT2004>> GetData(string BusinessParent, CancellationToken cancellationToken = default)
        {
            return await _businessContext.QueryAsync(new FilterQuery<BEMT2004>(m => m.BusinessParent == BusinessParent));
        }

        public Task SaveData(IEnumerable<BEMT2004> datas, CancellationToken cancellationToken = default)
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
