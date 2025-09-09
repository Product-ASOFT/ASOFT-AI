using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Business.Users.DataAccess.Interfaces;
using ASOFT.Core.Business.Users.DataAccsess.Interfaces;
using ASOFT.Core.Business.Users.Entities;
using ASOFT.Core.Business.Users.Entities.Requests;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using ASOFT.Core.Common.Security;
using ASOFT.Core.Common.Security.Identity.Jwt;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Entities;
using ASOFT.Core.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Business
{
    public class AuthenticationBusiness : IAuthenticationBusiness
    {
        private readonly IAuthenticationQueries _authenticationHelper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtHelper _jwtHelper;
        private readonly IScreenPermissionQueries _screenPermissionQueries;
        private readonly IMessageContext _messageContext;
        private readonly IMenuBusiness _menuBusiness;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICacheManagerBusiness _cacheService;
        private readonly IScreenPermissionBusiness _screenPermissionBusiness;
        private readonly IUserInfoQueries _userInfoQueries;
        private readonly IBusinessContext<AT1103> _at1103Context;
        public AuthenticationBusiness(IAuthenticationQueries authenticationHelper, IPasswordHasher passwordHasher,
            IJwtHelper jwtHelper, IScreenPermissionQueries screenPermissionQueries, IMessageContext messageContext,
            IMenuBusiness menuBusiness, IServiceScopeFactory serviceScopeFactory,
            ICacheManagerBusiness cacheService, IScreenPermissionBusiness screenPermissionBusiness, IUserInfoQueries userInfoQueries, IBusinessContext<AT1103> at1103Context)
        {
            _authenticationHelper =
                authenticationHelper ?? throw new ArgumentNullException(nameof(authenticationHelper));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _screenPermissionQueries = screenPermissionQueries ?? throw new ArgumentNullException(nameof(screenPermissionQueries));
            _messageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
            _menuBusiness = menuBusiness ?? throw new ArgumentNullException(nameof(menuBusiness));
            _menuBusiness = menuBusiness ?? throw new ArgumentNullException(nameof(menuBusiness));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _screenPermissionBusiness = screenPermissionBusiness ?? throw new ArgumentNullException(nameof(screenPermissionBusiness));
            _userInfoQueries = userInfoQueries ?? throw new ArgumentNullException(nameof(userInfoQueries));
            _at1103Context = at1103Context;
        }

        /// <summary>
        /// Sign in ERPX
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Clone từ SignIn và chỉnh sửa theo luồng đăng nhập ERPX
        /// </history>
        public async Task<AuthenticatedERPXModel> SignInERPX(SignInERPXRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Lấy thông tin user, bao gồm passwork mã hóa
            var userInfo = await _authenticationHelper.GetAuthenticationUserInfoAsync(request.UserID, cancellationToken);

            var hashedPassword = userInfo?.Password;
            if (string.IsNullOrEmpty(hashedPassword))
            {
                return null;
            }

            // Kiểm tra passwork có khớp
            if (_passwordHasher.VerifyHash(hashedPassword, request.Password))
            {
                var authenticatedModel = new AuthenticatedERPXModel();
                authenticatedModel.UserInfo = userInfo;
                authenticatedModel.UserInfo.LanguageID = request.LanguageID ?? "vi-VN";

                _ = _userInfoQueries.UpdateLanguageByUser(request.UserID, request.LanguageID, cancellationToken);

                var defaultDivisionModel = await _authenticationHelper.GetDefaultDivisionByUserIdAsync(
                                                        authenticatedModel.UserInfo.UserID,
                                                        cancellationToken) ?? new DivisionModel();
                // Tạo task lấy phân quyền màn hình, menu, first period, amount notification và mail setting
                var getPermissionTask = _screenPermissionBusiness.GetERPXScreenPermissionAsync(userInfo.UserID, userInfo.DivisionID, request.CustomerIndex, cancellationToken);
                var getMenuTask = _menuBusiness.GetMenuASOFTERPX(cancellationToken);
                var getFirstPeriodTask = _authenticationHelper.GetFirstPeriod(defaultDivisionModel.DivisionID, cancellationToken);
                var getAmountNotificationTask = _authenticationHelper.GetAmountNotification(request.UserID, defaultDivisionModel.DivisionID, cancellationToken);
                var getMailSettingReceivesTask = _authenticationHelper.GetMailSettingReceives(defaultDivisionModel.DivisionID, cancellationToken);

                // Chờ các task hoàn thành song song
                await Task.WhenAll(getPermissionTask, getMenuTask, getFirstPeriodTask, getAmountNotificationTask, getMailSettingReceivesTask);

                // Gán kết quả
                authenticatedModel.FirstPeriod = await getFirstPeriodTask;
                authenticatedModel.AmountNotification = await getAmountNotificationTask;
                authenticatedModel.SystemMailSettingReceives = await getMailSettingReceivesTask;
                authenticatedModel.ScreenPermissions = await getPermissionTask ?? throw new ArgumentNullException(nameof(getPermissionTask));
                authenticatedModel.Menu = await getMenuTask;

                authenticatedModel.DivisionID = defaultDivisionModel.DivisionID;
                authenticatedModel.DivisionName = defaultDivisionModel.DivisionName;

                // Tạo token bearer
                var claims = new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, authenticatedModel.UserInfo.UserID),
                        new Claim(ClaimTypes.Name, authenticatedModel.UserInfo.UserName)
                    };
                var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

                authenticatedModel.Token = _jwtHelper.CreateSecurityToken(DateTime.Now.AddDays(365), claimsIdentity.Claims);
                _ = _userInfoQueries.UpdateTokenByUser(authenticatedModel.UserInfo.UserID, authenticatedModel.Token, cancellationToken);

                return authenticatedModel;
            }

            return null;
        }


        public async Task<AuthenticatedModel> SignIn(SignInRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await _authenticationHelper.UseConnectionAsync(async connection =>
            {
                _authenticationHelper.SetConnection(connection);

                // Get hashed password from db
                var userInfo = await _authenticationHelper.GetAuthenticationUserInfoAsync(request.UserID,
                    cancellationToken);

                if (userInfo == null)
                {
                    userInfo = await _authenticationHelper.GetAuthenticationCustomer(request.UserID, cancellationToken);
                }

                var hashedPassword = userInfo?.Password;

                if (string.IsNullOrEmpty(hashedPassword))
                {
                    return null;
                }

                // Check password
                if (_passwordHasher.VerifyHash(hashedPassword, request.Password))
                {
                    var authenticatedModel = new AuthenticatedModel
                    {
                        GroupID = userInfo.GroupID,
                        LanguageID = userInfo.LanguageID ?? "vi-VN",
                        UserID = userInfo.UserID,
                        PageSize = userInfo.PageSize,
                        UserJoinRoleID = userInfo.UserJoinRoleID,
                        UserName = userInfo.UserName,
                        DeliveryAddress = userInfo.DeliveryAddress,
                        Email = userInfo.Email,
                        Tel = userInfo.Tel,
                        IsCustomer = userInfo.IsCustomer
                    };
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, authenticatedModel.UserID),
                        new Claim(ClaimTypes.Name, authenticatedModel.UserName)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

                    authenticatedModel.LanguageID = authenticatedModel.LanguageID ?? "vi-VN";
                    authenticatedModel.Token =
                        _jwtHelper.CreateSecurityToken(DateTime.Now.AddDays(365), claimsIdentity.Claims);

                    if (authenticatedModel.IsCustomer == 1)
                    {
                        authenticatedModel.DivisionIDList = new List<DivisionModel>();
                        authenticatedModel.DivisionID = userInfo.DivisionID;
                        authenticatedModel.DivisionID = userInfo.DivisionID;
                        authenticatedModel.ScreenPermissions = await _screenPermissionQueries.GetCustomerPermisson("ccm_permisson_groupid", userInfo.DivisionID, cancellationToken);
                    }
                    else
                    {
                        // Set divisionIds from Get Division by UserID
                        authenticatedModel.DivisionIDList = await _authenticationHelper.GetListDivisionByUserIdAsync(
                                                                authenticatedModel.UserID,
                                                                cancellationToken) ?? new List<DivisionModel>();

                        // First division is default division for user
                        //var firstItemDivision = authenticatedModel.DivisionIDList.FirstOrDefault();
                        //authenticatedModel.DivisionID = firstItemDivision?.DivisionID;
                        //authenticatedModel.DivisionName = firstItemDivision?.DivisionName;
                        var defaultDivisionModel = await _authenticationHelper.GetDefaultDivisionByUserIdAsync(
                                                                authenticatedModel.UserID,
                                                                cancellationToken) ?? new DivisionModel();

                        authenticatedModel.DivisionID = defaultDivisionModel.DivisionID;
                        authenticatedModel.DivisionName = defaultDivisionModel.DivisionName;

                        //Lấy dữ liệu phân quyền màn hình
                        var screenPermissions = await _screenPermissionQueries.GetScreenPermissionAsync(userInfo.UserID, userInfo.DivisionID, cancellationToken);
                        //Lấy cấu trúc menu
                        var menu = new Dictionary<string, AppMenu>();
                        if (request.IsAsoft == true)
                        {
                            menu = await _menuBusiness.GetMenuASOFT(userInfo.UserID, userInfo.DivisionID, screenPermissions, cancellationToken);
                        }
                        else
                        {
                            menu = await _menuBusiness.GetMenu(userInfo.UserID, userInfo.DivisionID, screenPermissions, cancellationToken);
                        }
                        authenticatedModel.ScreenPermissions = screenPermissions ?? throw new ArgumentNullException(nameof(screenPermissions));
                        authenticatedModel.Menu = menu;
                    }


                    _authenticationHelper.SetConnection(null);
                    return authenticatedModel;
                }

                _authenticationHelper.SetConnection(null);
                return null;
            }, cancellationToken);
        }

        public async Task<AuthenticatedModel> SignInBiometrics(SignInBiometricsRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await _authenticationHelper.UseConnectionAsync(async connection =>
            {
                _authenticationHelper.SetConnection(connection);

                // Lấy Publickey từ database theo UserID
                var userInfo = await _authenticationHelper.GetBiometricsKeyByUserId(request.UserID,
                   cancellationToken);

                var publicKey = userInfo?.BiometricsKey;

                if (string.IsNullOrEmpty(publicKey))
                {
                    return null;
                }

                bool isSignatureValid = false;
                using (var rsa = RSA.Create())
                {
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

                    byte[] payloadBytes = Encoding.UTF8.GetBytes(request.UserID);
                    byte[] signatureBytes = Convert.FromBase64String(request.Signature);

                    isSignatureValid = rsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }

                // Check chữ ký
                if (isSignatureValid)
                {
                    var authenticatedModel = new AuthenticatedModel
                    {
                        GroupID = userInfo.GroupID,
                        LanguageID = userInfo.LanguageID ?? "vi-VN",
                        UserID = userInfo.UserID,
                        PageSize = userInfo.PageSize,
                        UserJoinRoleID = userInfo.UserJoinRoleID,
                        UserName = userInfo.UserName,
                        DeliveryAddress = userInfo.DeliveryAddress,
                        Email = userInfo.Email,
                        Tel = userInfo.Tel,
                        IsCustomer = userInfo.IsCustomer
                    };
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, authenticatedModel.UserID),
                        new Claim(ClaimTypes.Name, authenticatedModel.UserName)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

                    authenticatedModel.LanguageID = authenticatedModel.LanguageID ?? "vi-VN";
                    authenticatedModel.Token =
                        _jwtHelper.CreateSecurityToken(DateTime.Now.AddDays(365), claimsIdentity.Claims);

                    if (authenticatedModel.IsCustomer == 1)
                    {
                        authenticatedModel.DivisionIDList = new List<DivisionModel>();
                        authenticatedModel.DivisionID = userInfo.DivisionID;
                        authenticatedModel.DivisionID = userInfo.DivisionID;
                        authenticatedModel.ScreenPermissions = await _screenPermissionQueries.GetCustomerPermisson("ccm_permisson_groupid", userInfo.DivisionID, cancellationToken);
                    }
                    else
                    {
                        // Set divisionIds from Get Division by UserID
                        authenticatedModel.DivisionIDList = await _authenticationHelper.GetListDivisionByUserIdAsync(
                                                                authenticatedModel.UserID,
                                                                cancellationToken) ?? new List<DivisionModel>();

                        // First division is default division for user
                        //var firstItemDivision = authenticatedModel.DivisionIDList.FirstOrDefault();
                        //authenticatedModel.DivisionID = firstItemDivision?.DivisionID;
                        //authenticatedModel.DivisionName = firstItemDivision?.DivisionName;
                        var defaultDivisionModel = await _authenticationHelper.GetDefaultDivisionByUserIdAsync(
                                                                authenticatedModel.UserID,
                                                                cancellationToken) ?? new DivisionModel();

                        authenticatedModel.DivisionID = defaultDivisionModel.DivisionID;
                        authenticatedModel.DivisionName = defaultDivisionModel.DivisionName;

                        //Lấy dữ liệu phân quyền màn hình
                        var screenPermissions = await _screenPermissionQueries.GetScreenPermissionAsync(userInfo.UserID, userInfo.DivisionID, cancellationToken);
                        //Lấy cấu trúc menu
                        var menu = new Dictionary<string, AppMenu>();
                        if (request.IsAsoft == true)
                        {
                            menu = await _menuBusiness.GetMenuASOFT(userInfo.UserID, userInfo.DivisionID, screenPermissions, cancellationToken);
                        }
                        else
                        {
                            menu = await _menuBusiness.GetMenu(userInfo.UserID, userInfo.DivisionID, screenPermissions, cancellationToken);
                        }
                        authenticatedModel.ScreenPermissions = screenPermissions ?? throw new ArgumentNullException(nameof(screenPermissions));
                        authenticatedModel.Menu = menu;
                    }


                    _authenticationHelper.SetConnection(null);
                    return authenticatedModel;
                }

                _authenticationHelper.SetConnection(null);
                return null;
            }, cancellationToken);
        }

        public async Task<bool> VerifyPassword(VerifyPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var culture = CultureInfo.CurrentUICulture;

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var userInfo = await _authenticationHelper.GetAuthenticationUserInfoAsync(request.UserID,
                    cancellationToken);

            if (!_passwordHasher.VerifyHash(userInfo.Password, request.Password))
            {
                return false;
            }
            return true;
        }

        public async Task<Result<bool, ErrorModelV2>> UpdateBiometricsKey(UpdateBiometricsKeyRequest request, CancellationToken cancellationToken = default)
        {
            var culture = CultureInfo.CurrentUICulture;

            var result = await _authenticationHelper.UpdateBiometricsKey(request.UserID, request.BiometricsKey, request.DivisionID);

            //Cập nhật thành công
            if (result > 0)
            {
                return Result<bool, ErrorModelV2>.FromSuccess(true);
            }

            //Lỗi
            var message1 = await _messageContext.GetByIDAsync("OOFML000084", culture.Name);
            return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: message1.Name));
        }

        public async Task<AuthenticatedModel> SignInERP9(SignInRequest request, bool isLoginQR, CancellationToken cancellationToken)
        {
            //SignInERP9Action(request, cancellationToken).Forget();
            _cacheService.ResetCacheByUserID(request.UserID);
            SignInERP991(request, isLoginQR, cancellationToken).Forget();
            return null;
        }

        /// <summary>
        /// Lấy thông tin userID khi ERP9.9 gọi qua
        /// Mục đích sinh ra hàm này là để lấy thông tin user khi login vào.
        /// Trường hợp có nhiều DivisionID
        /// Bổ sung thêm biến check login bằng mã QR
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isLoginQR"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthenticatedModel> SignInERP991(SignInRequest request, bool isLoginQR, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var authenticationHelper = scope.ServiceProvider.GetService<IAuthenticationQueries>();
                var passwordHasher = scope.ServiceProvider.GetService<IPasswordHasher>();
                var jwtHelper = scope.ServiceProvider.GetService<IJwtHelper>();
                var at1103 = scope.ServiceProvider.GetService<IBusinessContext<AT1103>>();

                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var auth = await authenticationHelper.UseConnectionAsync(async connection =>
                {
                    authenticationHelper.SetConnection(connection);

                    // Get hashed password from db
                    var userInfo = await _authenticationHelper.GetAuthenticationUserInfoAsync_v2(request.UserID,
                        cancellationToken);

                    if (userInfo == null)
                    {
                        return null;
                    }

                    var hashedPassword = userInfo?.Password;

                    if (string.IsNullOrEmpty(hashedPassword))
                    {
                        return null;
                    }

                    // Check password
                    if (passwordHasher.VerifyHash(hashedPassword, request.Password) || isLoginQR)
                    {
                        var authenticatedModel = new AuthenticatedModel
                        {
                            GroupID = userInfo.GroupID,
                            LanguageID = userInfo.LanguageID ?? "vi-VN",
                            UserID = userInfo.UserID,
                            PageSize = userInfo.PageSize,
                            UserJoinRoleID = userInfo.UserJoinRoleID,
                            UserName = userInfo.UserName,
                            DeliveryAddress = userInfo.DeliveryAddress,
                            Email = userInfo.Email,
                            Tel = userInfo.Tel,
                            IsCustomer = userInfo.IsCustomer
                        };
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, authenticatedModel.UserID),
                            new Claim(ClaimTypes.Name, authenticatedModel.UserName)
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

                        authenticatedModel.LanguageID = authenticatedModel.LanguageID ?? "vi-VN";
                        authenticatedModel.Token =
                            jwtHelper.CreateSecurityToken(DateTime.Now.AddDays(365), claimsIdentity.Claims);
                        authenticatedModel.DivisionIDList = await _authenticationHelper.GetListDivisionByUserIdAsync_v2(
                                                                   authenticatedModel.UserID,
                                                                   cancellationToken) ?? new List<DivisionModel>();

                        authenticationHelper.SetConnection(null);
                        return authenticatedModel;
                    }

                    authenticationHelper.SetConnection(null);
                    return null;
                }, cancellationToken);
                //var divisionIDList = new List<string>();
                //if (auth.DivisionIDList != null && auth.DivisionIDList.Any())
                //{
                //    divisionIDList = auth.DivisionIDList.Select(m => m.DivisionID).ToList();
                //}
                var divisionIDList = auth.DivisionIDList.Select(m => m.DivisionID).ToList();
                var entity = await at1103.QueryFirstOrDefaultAsync(new FilterQuery<AT1103>(m => m.EmployeeID == request.UserID && divisionIDList.Contains(m.DivisionID)));
                entity.TokenBearer = auth.Token;

                await at1103.UnitOfWork.ExecuteInTransactionAsync(async holder =>
                {
                    await at1103.UpdateAsync(entity);
                    await at1103.UnitOfWork.CompleteAsync();
                });

                return null;
            }
        }

        public async Task<AuthenticatedModel> ChangeDivison(string userID, string divisionID, CancellationToken cancellationToken)
        {

            return await _authenticationHelper.UseConnectionAsync(async connection =>
            {
                _authenticationHelper.SetConnection(connection);

                // Get hashed password from db
                var userInfo = await _authenticationHelper.GetAuthenticationUserInfoByDivision(userID, divisionID,
                    cancellationToken);
                if (userInfo == null)
                {
                    throw new ArgumentNullException(nameof(userInfo));
                }
                var authenticatedModel = new AuthenticatedModel
                {
                    GroupID = userInfo.GroupID,
                    LanguageID = userInfo.LanguageID ?? "vi-VN",
                    UserID = userInfo.UserID,
                    PageSize = userInfo.PageSize,
                    UserJoinRoleID = userInfo.UserJoinRoleID,
                    UserName = userInfo.UserName
                };

                var claims = new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, authenticatedModel.UserID),
                        new Claim(ClaimTypes.Name, authenticatedModel.UserName)
                    };
                var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

                authenticatedModel.LanguageID = authenticatedModel.LanguageID ?? "vi-VN";
                authenticatedModel.Token =
                    _jwtHelper.CreateSecurityToken(DateTime.Now.AddDays(365), claimsIdentity.Claims);

                // Set divisionIds from Get Division by UserID
                authenticatedModel.DivisionIDList = await _authenticationHelper.GetListDivisionByUserIdAsync(
                                                        authenticatedModel.UserID,
                                                        cancellationToken) ?? new List<DivisionModel>();


                // First division is default division for user
                var firstItemDivision = authenticatedModel.DivisionIDList.Where(m => m.DivisionID == divisionID).FirstOrDefault();
                authenticatedModel.DivisionID = firstItemDivision?.DivisionID;
                authenticatedModel.DivisionName = firstItemDivision?.DivisionName;

                //Lấy dữ liệu phân quyền màn hình
                var screenPermissions = await _screenPermissionQueries.GetScreenPermissionAsync(userInfo.UserID, userInfo.DivisionID, cancellationToken);

                authenticatedModel.ScreenPermissions = screenPermissions ?? throw new ArgumentNullException(nameof(screenPermissions));

                _authenticationHelper.SetConnection(null);
                return authenticatedModel;


            }, cancellationToken);
        }

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="divisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<bool, ErrorModelV2>> ChangePassword(string userID, string oldPassword, string newPassword, string divisionID, string deviceID = null, CancellationToken cancellationToken = default)
        {
            var culture = CultureInfo.CurrentUICulture;

            //Kiểm tra mật khẩu mới có đúng cấu trúc
            var regexCheck = new Regex("^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9]).{8,50}$").IsMatch(newPassword);
            if (!regexCheck)
            {
                var message = await _messageContext.GetByIDAsync("00ML000208", culture.Name);
                return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: message.Name));
            }

            //Mật khẫu cũ không đúng
            var userInfo = await _authenticationHelper.GetAuthenticationUserInfoAsync(userID,
                    cancellationToken);
            if (!_passwordHasher.VerifyHash(userInfo.Password, oldPassword))
            {
                var message = await _messageContext.GetByIDAsync("ASML000019", culture.Name);
                return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: message.Name));
            }

            var hashedNewPass = _passwordHasher.Encrypt(newPassword);
            var result = await _authenticationHelper.ChangePassword(userID, hashedNewPass, oldPassword, divisionID, deviceID);


            //Cập nhật thành công
            if (result > 0)
            {
                return Result<bool, ErrorModelV2>.FromSuccess(true);
            }

            //Lỗi
            var message1 = await _messageContext.GetByIDAsync("OOFML000084", culture.Name);
            return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: message1.Name));
        }
    }
}