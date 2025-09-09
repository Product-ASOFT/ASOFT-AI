using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;

namespace ASOFT.Authentication.API.OAuthentication2.Store
{
    public class ASOFTReferenceTokenStore : DefaultReferenceTokenStore
    {
        public ASOFTReferenceTokenStore(IPersistedGrantStore store, IPersistentGrantSerializer serializer,
            IHandleGenerationService handleGenerationService, ILogger<DefaultReferenceTokenStore> logger) : base(store,
            serializer, handleGenerationService, logger)
        {

        }

        protected override string GetHashedKey(string value)
        {
            return base.GetHashedKey(value);
        }
    }
}