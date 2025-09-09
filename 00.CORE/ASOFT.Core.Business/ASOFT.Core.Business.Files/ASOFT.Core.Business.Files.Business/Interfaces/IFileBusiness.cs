using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Files.Entities.Requests;
using ASOFT.Core.Business.Files.Entities.Respones;
using ASOFT.Core.DataAccess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Files.Business.Interfaces
{
    public interface IFileBusiness
    {
        /// <summary>
        /// Xử lý xóa file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Result<bool, ErrorModelV2>> RemoveFile(RemoveFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Xử lý upload file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UploadRespone> UploadFile(UploadFileRequest request, CancellationToken cancellationToken);
    }
}
