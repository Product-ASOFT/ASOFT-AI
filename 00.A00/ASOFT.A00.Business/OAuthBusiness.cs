using ASOFT.A00.Business.Interfaces;
using ASOFT.A00.Entities;
using ASOFT.A00.Entities.ViewModels;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.Business
{
    public class OAuthBusiness : IOAuthBusiness
    {
        private readonly IBusinessContext<IOTT1010> _iott1010Context;
        private readonly IBusinessContext<IOTT1011> _iott1011Context;

        public OAuthBusiness(IBusinessContext<IOTT1010> iott1010Context, IBusinessContext<IOTT1011> iott1011Context)
        {
            _iott1010Context = iott1010Context;
            _iott1011Context = iott1011Context;
        }

        public async Task<string> UpdateAccessToken(string clinetDomain, string refeshToken, string iotHost = null, CancellationToken cancellationToken = default)
        {
            var accessToken = "at" + Guid.NewGuid().ToString();
            var accountLinking = await _iott1010Context.QueryFirstOrDefaultAsync(new FilterQuery<IOTT1010>(m => m.ClientDomain == clinetDomain));

            var now = DateTime.Now;
            //linking mới
            if (accountLinking == null)
            {
                await _iott1010Context.UnitOfWork.ExecuteInTransactionAsync(async holder =>
                {
                    var entity = new IOTT1010
                    {
                        ClientDomain = clinetDomain,
                        CreateDate = now,
                        DivisionID = "@@@",
                        RefeshToken = refeshToken,
                        IOTHost = iotHost,
                        LastModifyDate = now,
                    };
                    await _iott1010Context.AddAsync(entity, cancellationToken);
                    await _iott1011Context.AddAsync(new IOTT1011
                    {
                        AccessToken = accessToken,
                        APK = Guid.NewGuid(),
                        ClientDomain = clinetDomain,
                        CreateDate = now,
                        ExpireDate = now.AddDays(365),
                        IOTHost = iotHost
                    });
                    await _iott1010Context.UnitOfWork.CompleteAsync();
                    return true;
                });              
            }
            //Trường hợp relink
            else
            {
                await _iott1010Context.UnitOfWork.ExecuteInTransactionAsync(async holder =>
                {
                    accountLinking.LastModifyDate = DateTime.Now;
                    accountLinking.RefeshToken = refeshToken;
                    accountLinking.IsRenew = 0;
                    await _iott1010Context.UpdateAsync(accountLinking, cancellationToken);
                    await _iott1011Context.AddAsync(new IOTT1011
                    {
                        AccessToken = accessToken,
                        APK = Guid.NewGuid(),
                        ClientDomain = clinetDomain,
                        CreateDate = now,
                        ExpireDate = now.AddDays(365),
                        IOTHost = iotHost
                    });
                    await _iott1010Context.UnitOfWork.CompleteAsync();
                    return true;
                });              
            }
            return accessToken;
        }

        public async Task<string> RenewAccessToken(string refeshToken, CancellationToken cancellationToken = default)
        {
            var accountLinking = await _iott1010Context.QueryFirstOrDefaultAsync(new FilterQuery<IOTT1010>(m => m.RefeshToken == refeshToken));
            if (accountLinking == null) return null;
            var accessToken = Guid.NewGuid().ToString();
            return await _iott1010Context.UnitOfWork.ExecuteInTransactionAsync(async holder =>
            {
                accountLinking.LastModifyDate = DateTime.Now;
                accountLinking.IsRenew = 1;
                var now = DateTime.Now;
                await _iott1011Context.AddAsync(new IOTT1011
                {
                    AccessToken = accessToken,
                    APK = Guid.NewGuid(),
                    ClientDomain = accountLinking.ClientDomain,
                    CreateDate = now,
                    ExpireDate = now.AddDays(365),
                    IOTHost = accountLinking.IOTHost
                });
                await _iott1010Context.UpdateAsync(accountLinking, cancellationToken);
                await _iott1010Context.UnitOfWork.CompleteAsync();
                return accessToken;
            });
        }

        public async Task<IoTTokenResponse> ValidateAccessToken(string accessToken, CancellationToken cancellationToken = default)
        {
            var accountLinking = await _iott1011Context.QueryFirstOrDefaultAsync(new FilterQuery<IOTT1011>(m => m.AccessToken == accessToken && m.ExpireDate >= DateTime.Now));
            if (accountLinking == null) return null;
            return new IoTTokenResponse {
                Domain = accountLinking.ClientDomain,
                Host = accountLinking.IOTHost
            };
        }
    }
}
