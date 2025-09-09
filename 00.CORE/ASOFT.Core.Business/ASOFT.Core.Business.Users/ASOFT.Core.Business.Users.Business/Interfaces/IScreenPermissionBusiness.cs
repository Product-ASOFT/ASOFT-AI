using ASOFT.Core.Business.Common.Entities.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Business.Interfaces
{
    public interface IScreenPermissionBusiness
    {
        Task<ConcurrentDictionary<string, ASOFTPermission>> GetERPXScreenPermissionAsync(string userID, string divisionID, int customerIndex, CancellationToken cancellationToken = default);
    }
}
