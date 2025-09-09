using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2121Queries
    {
        /// <summary>
        /// Thêm mới một prompt cho agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CreateFileResult(ST2121 result, CancellationToken cancellationToken = default);

        Task<bool> CreateFileResult(IEnumerable<ST2121> readFileResults, CancellationToken cancellationToken = default);
    }
}