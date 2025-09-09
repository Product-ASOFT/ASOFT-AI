using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public class ST2121Queries : IST2121Queries
    {
        private readonly IBusinessContext<ST2121> _businessContext;

        public ST2121Queries(IBusinessContext<ST2121> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }

        public async Task<ST2121> QueryPromptsByAgentCode(int AattachID)
        {
            var readFileResult = await _businessContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2121>(m => m.AttachID == AattachID));
            return readFileResult;
        }

        public async Task<bool> CreateFileResult(ST2121 readFileResult, CancellationToken cancellationToken = default)
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

        public async Task<bool> CreateFileResult(IEnumerable<ST2121> readFileResults, CancellationToken cancellationToken = default)
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
    }
}