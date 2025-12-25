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
    public class ST2137Queries : IST2137Queries
    {

        private readonly IBusinessContext<ST2137> _businessContext;

        public ST2137Queries(IBusinessContext<ST2137> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }
        public Task SaveData(IEnumerable<ST2137> datas, CancellationToken cancellationToken = default)
        {
            return _businessContext.UnitOfWork.ExecuteInTransactionAsync(
            async transactionHolder =>
            {
                await _businessContext.AddRangeAsync(datas, cancellationToken);
                await _businessContext.UnitOfWork.CompleteAsync();
            });
        }
        public async Task<IEnumerable<ST2137>> GetData(Guid apkMater_ST2131)
        {
            return await _businessContext.QueryAsync(new FilterQuery<ST2137>(m => m.APKMaster_ST2131 == apkMater_ST2131));
        }
        public async Task<bool> DeleteData(Guid apkMater_ST2131, CancellationToken cancellationToken = default)
        {
            var datas = await GetData(apkMater_ST2131);
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
