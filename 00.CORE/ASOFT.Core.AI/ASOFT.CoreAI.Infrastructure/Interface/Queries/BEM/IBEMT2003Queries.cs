using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IBEMT2003Queries
    {
        /// <summary>
        /// Thêm mới một prompt cho agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveData(BEMT2003 data, CancellationToken cancellationToken = default);

        Task<bool> CreateData(IEnumerable<BEMT2003> datas, CancellationToken cancellationToken = default);

        Task UpdateData(BEMT2003 data, CancellationToken cancellationToken = default);
        Task<BEMT2003> GetData(Guid APK);
        Task<BEMT2003> GetDataByAPKMaster(Guid APKMaster);

        Task<bool> DeleteData(Guid APKMaster, CancellationToken cancellationToken = default);
    }
}