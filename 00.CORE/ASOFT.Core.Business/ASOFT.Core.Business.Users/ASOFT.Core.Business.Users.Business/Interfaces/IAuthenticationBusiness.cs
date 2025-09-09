using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Users.Entities;
using ASOFT.Core.Business.Users.Entities.Requests;
using ASOFT.Core.DataAccess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Business.Interfaces
{
    public interface IAuthenticationBusiness
    {
        Task<AuthenticatedERPXModel> SignInERPX(SignInERPXRequest request, CancellationToken cancellationToken);
        Task<AuthenticatedModel> SignIn(SignInRequest request, CancellationToken cancellationToken);
        Task<AuthenticatedModel> SignInBiometrics(SignInBiometricsRequest request, CancellationToken cancellationToken);
        Task<bool> VerifyPassword(VerifyPasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool, ErrorModelV2>> UpdateBiometricsKey(UpdateBiometricsKeyRequest request, CancellationToken cancellationToken = default);
        Task<AuthenticatedModel> ChangeDivison(string divisionID, string userID, CancellationToken cancellationToken);
        Task<Result<bool, ErrorModelV2>> ChangePassword(string userID, string oldPassword, string newPassword, string divisionID,string deviceID = null, CancellationToken cancellationToken = default);
        Task<AuthenticatedModel> SignInERP9(SignInRequest request, bool isLoginQR, CancellationToken cancellationToken);
    }
}
