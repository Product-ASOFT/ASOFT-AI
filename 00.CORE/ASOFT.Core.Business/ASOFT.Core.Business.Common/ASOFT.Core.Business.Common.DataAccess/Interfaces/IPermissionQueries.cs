using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Common.Requests;
using ASOFT.Core.DataAccess.Relational;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    /// <summary>
    /// Helper class permission
    /// </summary>
    public interface IPermissionQueries : IRelationDataExecutor
    {
        /// <summary>
        /// Lấy thông tin permission
        /// </summary>
        /// <param name="params"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PermissionCondition> GetPermissionConditionAsync(PermissionRequest @params,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy thông tin permission theo screen
        /// </summary>
        /// <param name="params"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ScreenPermission> GetPermissionByScreenAsync(ScreenPermissionRequest @params,
            CancellationToken cancellationToken = default);
    }
}