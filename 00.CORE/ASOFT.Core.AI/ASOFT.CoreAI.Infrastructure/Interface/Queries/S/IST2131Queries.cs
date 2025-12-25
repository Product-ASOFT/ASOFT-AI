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
        Task<bool> SaveData(ST2131 data, CancellationToken cancellationToken = default);

        Task<bool> CreateData(IEnumerable<ST2131> datas, CancellationToken cancellationToken = default);

        Task UpdateData(ST2131 data, CancellationToken cancellationToken = default);
        Task<ST2131> GetData(Guid APK);
        Task<ST2131> GetDataByAPKMaster(Guid APKMaster);

        Task<bool> DeleteData(ST2131 data, CancellationToken cancellationToken = default);
    }
}