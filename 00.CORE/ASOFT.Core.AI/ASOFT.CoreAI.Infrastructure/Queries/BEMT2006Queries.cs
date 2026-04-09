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
    public class BEMT2006Queries : IBEMT2006Queries
    {

        private readonly IBusinessContext<BEMT2006> _businessContext;

        public BEMT2006Queries(IBusinessContext<BEMT2006> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }
        public Task SaveData(IEnumerable<BEMT2006> datas, CancellationToken cancellationToken = default)
        {
            return _businessContext.UnitOfWork.ExecuteInTransactionAsync(
               async transactionHolder =>
               {
                   await _businessContext.AddRangeAsync(datas, cancellationToken);
                   await _businessContext.UnitOfWork.CompleteAsync();
               });
        }
        public async Task<IEnumerable<BEMT2006>> GetData(Guid apkMater_BEMT2003)
        {
            return await _businessContext.QueryAsync(new FilterQuery<BEMT2006>(m => m.APKMaster == apkMater_BEMT2003));
        }
        public async Task<bool> DeleteData(Guid apkMater_BEMT2003, CancellationToken cancellationToken = default)
        {
            var datas = await GetData(apkMater_BEMT2003);
            if (datas == null)
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
