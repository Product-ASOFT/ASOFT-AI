// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    09/07/2020    Thành Luân          Bổ sung tài liệu
// ##################################################################

using ASOFT.API.Core.ApiResponse;
using ASOFT.API.Core.Errors;
using ASOFT.Authentication.API.OAuthentication2.Models;
using ASOFT.Contract;
using ASOFT.Security.Core;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ASOFT.Authentication.API.OAuthentication2.Validators
{
    /// <summary>
    /// Validator password for user
    /// </summary>
    public class ASOFTResourcesOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ITemporaryUserHolder _temporaryUserHolder;
        private readonly ISystemClock _systemClock;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthenticationDataAccess _authenticationDataAccess;

        public ASOFTResourcesOwnerPasswordValidator(ITemporaryUserHolder temporaryUserHolder, ISystemClock systemClock,
            IPasswordHasher passwordHasher, IAuthenticationDataAccess authenticationDataAccess)
        {
            _temporaryUserHolder = Checker.NotNull(temporaryUserHolder, nameof(temporaryUserHolder));
            _passwordHasher = Checker.NotNull(passwordHasher, nameof(passwordHasher));
            _systemClock = Checker.NotNull(systemClock, nameof(systemClock));
            _authenticationDataAccess = Checker.NotNull(authenticationDataAccess, nameof(authenticationDataAccess));
        }

        public virtual async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            // Get user
            var user = await _authenticationDataAccess.GetUserByIDAsync(context.UserName);

            // If invalid
            if (user == null || string.IsNullOrEmpty(user.Password) ||
                !_passwordHasher.VerifyHash(user.Password, context.Password))
            {
                // Convert error model to dictionary
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, null,
                    CreateDictionaryFromErrorModel(
                        new ErrorResponse<ErrorModel>(new ErrorModel(CoreErrorCodes.UserNamePasswordInvalid.Value))));
                return;
            }

            // Validation success
            // Set user to scope request for use in another object.
            _temporaryUserHolder.SetUser(user);
            context.Result = new GrantValidationResult(user.UserID, OidcConstants.AuthenticationMethods.Password,
                _systemClock.UtcNow.UtcDateTime);
        }

        private static Dictionary<string, object> CreateDictionaryFromErrorModel(
            ErrorResponse<ErrorModel> errorResponse)
        {
            return errorResponse.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(errorResponse));
        }
    }
}