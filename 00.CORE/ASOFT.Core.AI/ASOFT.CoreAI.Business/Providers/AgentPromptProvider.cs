using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class AgentPromptProvider
    {
        private IST2111Queries _agentPromptQueries;

        public AgentPromptProvider(IST2111Queries agentPromptQueries)
        {
            _agentPromptQueries = agentPromptQueries;
        }

        public async Task<ST2111> GetPromptContentAsync(string typePrompt)
        {
            var prompt = await _agentPromptQueries.QueryPromptsByAgentCode(typePrompt);
            if (prompt == null || string.IsNullOrEmpty(prompt.PromptContent))
            {
                return null;
            }
            return prompt;
        }
    }
}