using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public class ST2111Queries : IST2111Queries
    {
        private readonly IBusinessContext<ST2111> _agentPromptContext;

        public ST2111Queries(IBusinessContext<ST2111> agentPromptContext)
        {
            _agentPromptContext = Checker.NotNull(agentPromptContext, nameof(agentPromptContext));
        }

        public async Task<ST2111> QueryPromptsByAgentCode(string agentCode, CancellationToken cancellationToken = default)
        {
            var prompt = await _agentPromptContext.QueryFirstOrDefaultAsync(new FilterQuery<ST2111>(m => m.AgentCode == agentCode));
            return prompt;
        }

        public async Task<bool> CreateAgentPrompt(ST2111 agent, CancellationToken cancellationToken = default)
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