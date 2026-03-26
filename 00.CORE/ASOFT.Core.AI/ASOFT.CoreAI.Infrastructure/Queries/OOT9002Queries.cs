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
    public class OOT9002Queries : IOOT9002Queries
    {
        private readonly IBusinessContext<OOT9002> _businessContext;

        public OOT9002Queries(IBusinessContext<OOT9002> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }

        public Task<bool> SaveData(OOT9002 data, CancellationToken cancellationToken = default)
        {
            try
            {
                return _businessContext.UnitOfWork.ExecuteInTransactionAsync(
                async transactionHolder =>
                {
                    await _businessContext.AddAsync(data, cancellationToken);
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
