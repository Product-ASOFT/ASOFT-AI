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
        public async Task<bool> SaveData(ST2131 data, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _businessContext.AddAsync(data, cancellationToken);
                    await _businessContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CreateData(IEnumerable<ST2131> datas, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _businessContext.AddRangeAsync(datas, cancellationToken);
                    await _businessContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateData(ST2131 data, CancellationToken cancellationToken = default)
        {
            await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
           {
               await _businessContext.UpdateAsync(data, cancellationToken);
               await _businessContext.UnitOfWork.CompleteAsync();
           });
        }
        public async Task<ST2131> GetData(Guid APK)
        {
            return await _businessContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2131>(m => m.APK == APK));
        }
    }
}