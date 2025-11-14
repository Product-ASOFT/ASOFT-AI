using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IST2131Queries
    {
        /// <summary>
        /// Thêm mới một prompt cho agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveFileResult(ST2131 result, CancellationToken cancellationToken = default);

        Task<bool> CreateFileResult(IEnumerable<ST2131> readFileResults, CancellationToken cancellationToken = default);

        Task<bool> UpdateFileResult(ST2131 readFileResult, CancellationToken cancellationToken = default);
        Task<ST2131> GetFileResult(Guid APK);
    }
}