using IdentityServer4.Services;
using System.Threading.Tasks;

namespace ASOFT.Authentication.API.OAuthentication2.Services
{
    public class ASOFTHandleGenerationService : IHandleGenerationService
    {
        public async Task<string> GenerateAsync(int length = 32)
        {
            throw new System.NotImplementedException();
        }
    }
}