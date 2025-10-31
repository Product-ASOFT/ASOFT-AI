using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2130Queries
    {
        /// <summary>
        /// Lấy danh sách các prompt của agent
        /// </summary>
        /// <param name="agentCode">Mã agent</param>
        /// <param name="moduleCode">Mã module (nếu có)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Danh sách các prompt</returns>
        Task<ST2130> QueryPromptsByAgentCode(string agentCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới một prompt cho agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CreateAgentPrompt(ST2130 agent, CancellationToken cancellationToken = default);
    }
}