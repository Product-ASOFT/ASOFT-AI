using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Business.Common.Entities.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business.Interfaces
{
    public interface ICommentsBusiness
    {
        /// <summary>
        /// Thêm mới ghi chú
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CRMT90031_REL> InsertComment(CreateCommentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa ghi chú
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> RemoveComment(RemoveCommentRequest request, CancellationToken cancellationToken);
    }
}
