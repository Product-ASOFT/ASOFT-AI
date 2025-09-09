using ASOFT.AT.Entity;
using JetBrains.Annotations;

namespace ASOFT.Authentication.API.OAuthentication2.Models
{
    public interface ITemporaryUserHolder
    {
        void SetUser(AT1405 user);

        [CanBeNull]
        AT1405 GetUser();
    }
}