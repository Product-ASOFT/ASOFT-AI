using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public class ST2130Queries : IST2130Queries
    {
        private readonly IBusinessContext<ST2130> _agentPromptContext;

        public ST2130Queries(IBusinessContext<ST2130> agentPromptContext)
        {
            _agentPromptContext = Checker.NotNull(agentPromptContext, nameof(agentPromptContext));
        }

        public async Task<ST2130> QueryPromptsByAgentCode(string agentCode, CancellationToken cancellationToken = default)
        {
            try
            {
                var prompt = await _agentPromptContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2130>(m => m.AgentCode == agentCode));
                return prompt;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> CreateAgentPrompt(ST2130 agent, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _agentPromptContext.UnitOfWork.ExecuteInTransactionAsync(async (transactionHolder) =>
                {
                    await _agentPromptContext.AddAsync(agent, cancellationToken);
                    await _agentPromptContext.UnitOfWork.CompleteAsync();
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