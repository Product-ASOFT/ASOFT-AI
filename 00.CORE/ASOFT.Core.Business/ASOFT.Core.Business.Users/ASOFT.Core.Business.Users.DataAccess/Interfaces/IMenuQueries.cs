using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Interfaces
{
    public interface IMenuQueries
    {
        Task<IEnumerable<AppMenu>> GetMenu(CancellationToken cancellationToken = default);
        Task<IEnumerable<AppMenu>> GetMenuASOFT(CancellationToken cancellationToken = default);
        Task<IEnumerable<ERPXMenu>> GetMenuASOFTERPX(CancellationToken cancellationToken = default);
    }
}
