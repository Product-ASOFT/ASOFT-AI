using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Business.Interfaces
{
    public interface  IMenuBusiness
    {

        Task<Dictionary<string, AppMenu>> GetMenu(string userID, string divisionID, CancellationToken cancellationToken = default);

        Task<Dictionary<string, AppMenu>> GetMenu(string userID, string divisionID, IEnumerable<AP1403ViewModel> permissions, CancellationToken cancellationToken = default);
        Task<Dictionary<string, AppMenu>> GetMenuASOFT(string userID, string divisionID, IEnumerable<AP1403ViewModel> permissions, CancellationToken cancellationToken = default);
        Task<Dictionary<string, AppMenu>> GetMenu(CancellationToken cancellationToken = default);

        Task<IEnumerable<ERPXMenu>> GetMenuASOFTERPX(CancellationToken cancellationToken = default);

    }
}
