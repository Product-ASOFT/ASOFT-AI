using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2111Queries
    {
        /// <summary>
        /// Lấy danh sách các prompt của agent
        /// </summary>
        /// <param name="agentCode">Mã agent</param>
        /// <param name="moduleCode">Mã module (nếu có)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Danh sách các prompt</returns>
        Task<ST2111> QueryPromptsByAgentCode(string agentCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới một prompt cho agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CreateAgentPrompt(ST2111 agent, CancellationToken cancellationToken = default);
    }
}