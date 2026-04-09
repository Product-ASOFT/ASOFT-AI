using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public class BEMT2003Queries : IBEMT2003Queries
    {
        private readonly IBusinessContext<BEMT2003> _businessContext;

        public BEMT2003Queries(IBusinessContext<BEMT2003> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }
        public async Task<bool> SaveData(BEMT2003 data, CancellationToken cancellationToken = default)
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

        public async Task<bool> CreateData(IEnumerable<BEMT2003> datas, CancellationToken cancellationToken = default)
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
        public async Task UpdateData(BEMT2003 data, CancellationToken cancellationToken = default)
        {
            await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
           {
               await _businessContext.UpdateAsync(data, cancellationToken);
               await _businessContext.UnitOfWork.CompleteAsync();
           });
        }
        public async Task<BEMT2003> GetData(Guid APK)
        {
            return await _businessContext.QueryFirstOrDefaultAsync(new FilterQuery<BEMT2003>(m => m.APK == APK));
        }
        public async Task<List<BEMT2003>> GetAllDataByAPK(Guid APKMaster)
        {
            return await _businessContext.QueryAsync(new FilterQuery<BEMT2003>(m => m.APKMaster == APKMaster));
        }
        public async Task<BEMT2003> GetDataByAPKMaster(Guid APKMaster)
        {
            return await _businessContext.QueryFirstOrDefaultAsync(new FilterQuery<BEMT2003>(m => m.APKMaster == APKMaster));
        }
        public async Task<bool> DeleteData(Guid APKMaster, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataDelete = await GetAllDataByAPK(APKMaster);
                if (dataDelete == null || dataDelete.Count() == 0)
                {
                    return true;
                }
                return await _businessContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _businessContext.BulkDeleteAsync(dataDelete);
                    await _businessContext.UnitOfWork.CompleteAsync();
                    return true;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}