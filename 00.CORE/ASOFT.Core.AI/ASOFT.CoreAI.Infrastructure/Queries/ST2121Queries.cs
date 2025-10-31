using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public class ST2121Queries : IST2131Queries
    {
        private readonly IBusinessContext<ST2131> _businessContext;

        public ST2121Queries(IBusinessContext<ST2131> businessContext)
        {
            _businessContext = Checker.NotNull(businessContext, nameof(businessContext));
        }

        public async Task<ST2131> QueryPromptsByAgentCode(int AattachID)
        {
            var readFileResult = await _businessContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2131>(m => m.AttachID == AattachID));
            return readFileResult;
        }

        public async Task<bool> CreateFileResult(ST2131 readFileResult, CancellationToken cancellationToken = default)
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
    }
}