using ASOFT.Authentication.Domain.Aggregates.AuthenticationAggregate;
using ASOFT.Contract;

namespace ASOFT.Authentication.API.OAuthentication2.Models
{
    public class TemporaryUserHolder : ITemporaryUserHolder
    {
        private AuthenticationUserInfo _user;

        public void SetUser(AT1405 user)
        {
            Checker.NotNull(user, nameof(user));
            _user = user;
        }

        public AT1405 GetUser() => _user;
    }
}