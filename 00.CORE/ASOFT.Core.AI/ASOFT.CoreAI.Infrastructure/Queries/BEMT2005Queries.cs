using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public class BEMT2005Queries : IBEMT2005Queries
    {

        private readonly IBusinessContext<BEMT2005> _businessContext;

        public BEMT2005Queries(IBusinessContext<BEMT2005> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }
        public Task SaveData(IEnumerable<BEMT2005> datas, CancellationToken cancellationToken = default)
        {
            return _businessContext.UnitOfWork.ExecuteInTransactionAsync(
            async transactionHolder =>
            {
                await _businessContext.AddRangeAsync(datas, cancellationToken);
                await _businessContext.UnitOfWork.CompleteAsync();
            });
        }
        public async Task<IEnumerable<BEMT2005>> GetData(Guid apkMater_BEMT2003)
        {
            return await _businessContext.QueryAsync(new FilterQuery<BEMT2005>(m => m.APKMaster == apkMater_BEMT2003));
        }
        public async Task<bool> DeleteData(Guid apkMater_BEMT2003, CancellationToken cancellationToken = default)
        {
            var datas = await GetData(apkMater_BEMT2003);
            if(datas == null)
                return true;
            return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async tran =>
            {
                await _businessContext.BulkDeleteAsync(datas);
                await _businessContext.UnitOfWork.CompleteAsync();
                return true;
            });
        }
    }
}
