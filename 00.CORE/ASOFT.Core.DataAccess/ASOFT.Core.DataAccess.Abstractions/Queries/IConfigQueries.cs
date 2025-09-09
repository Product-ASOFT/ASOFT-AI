using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Data.Core.Queries
{
    public interface IConfigQueries
    {
        /// <summary>
        /// Lấy giá trị thiết lập từ DB theo group và tên
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="keyName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetConfigValue(int groupID, string keyName, CancellationToken cancellationToken = default);
    }
}
