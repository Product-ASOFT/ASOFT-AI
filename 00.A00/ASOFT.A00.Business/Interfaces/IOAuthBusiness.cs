using ASOFT.A00.Entities.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.Business.Interfaces
{
    public interface IOAuthBusiness
    {
        Task<string> UpdateAccessToken(string clinetDomain, string refeshToken, string iotHost = null, CancellationToken cancellationToken = default);
        Task<string> RenewAccessToken(string refeshToken, CancellationToken cancellationToken = default);
        Task<IoTTokenResponse> ValidateAccessToken(string accessToken, CancellationToken cancellationToken = default);
    }
}
