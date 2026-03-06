using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2130Queries
    {
        /// <summary>
        /// Lấy danh sách các prompt của agent
        /// </summary>
        /// <param name="agentCode">Mã agent</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Danh sách các prompt</returns>
        Task<ST2130> GetPromptByCode(string agentCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới một prompt cho agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CreateAgentPrompt(ST2130 agent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy prompt theo mã agent và loại so sánh  
        /// </summary>
        /// <param name="agentCode"></param>
        /// <param name="typeCompare"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ST2130> GetPromptByTypeCompare(string agentCode, string typeCompare, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm mới nhiều prompt cho agent
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CreateListAgentPrompt(IEnumerable<ST2130> agents, CancellationToken cancellationToken = default);

        Task<ST2130> GetPromptByTypePrompt(string agentCode, string typeCompare, CancellationToken cancellationToken = default);
        Task<List<ST2130>> GetPromptsByAgentCodeAndTypeCompare(string agentCode, string typeCompare, CancellationToken cancellationToken = default);
    }
}