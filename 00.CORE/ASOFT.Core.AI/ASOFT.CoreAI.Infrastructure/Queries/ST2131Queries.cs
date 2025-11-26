using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public class ST2131Queries : IST2131Queries
    {
        private readonly IBusinessContext<ST2131> _businessContext;

        public ST2131Queries(IBusinessContext<ST2131> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }
        public async Task<bool> SaveFileResult(ST2131 readFileResult, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _businessContext.AddAsync(readFileResult, cancellationToken);
                    await _businessContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CreateFileResult(IEnumerable<ST2131> readFileResults, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _businessContext.AddRangeAsync(readFileResults, cancellationToken);
                    await _businessContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateFileResult(ST2131 readFileResult, CancellationToken cancellationToken = default)
        {
            await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
           {
               await _businessContext.UpdateAsync(readFileResult, cancellationToken);
               await _businessContext.UnitOfWork.CompleteAsync();
           });
        }

        public async Task<ST2131> GetFileResult(Guid APK)
        {
            return await _businessContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2131>(m => m.APK == APK));
        }
    }
}