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
    public class OOT9003Queries : IOOT9003Queries
    {
        private readonly IBusinessContext<OOT9003> _businessContext;

        public OOT9003Queries(IBusinessContext<OOT9003> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }
        public Task<bool> SaveData(IEnumerable<OOT9003> datas, CancellationToken cancellationToken = default)
        {
            try
            {
                return _businessContext.UnitOfWork.ExecuteInTransactionAsync(
                async transactionHolder =>
                {
                    await _businessContext.AddRangeAsync(datas, cancellationToken);
                    await _businessContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {

                return Task.FromResult(false);
            }
        }
    }
}
