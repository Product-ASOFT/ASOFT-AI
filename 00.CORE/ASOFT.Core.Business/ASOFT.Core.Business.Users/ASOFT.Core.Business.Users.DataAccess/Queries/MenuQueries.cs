using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using ASOFT.Core.DataAccess;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Queries
{
    public class MenuQueries : AdminDataAccess, IMenuQueries
    {
        public MenuQueries(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        private const string SQLGetMenu = @"
                                SELECT  MenuText, MenuAppID, sysMenuID, MenuLevel, sysMenuParent, MenuOrder, ModuleID
                                FROM            sysMenu WITH (NOLOCK)
                                WHERE   (MenuAppID IS NOT NULL) and CustomerIndex = -1
                                ";

        private const string SQLGetMenuASOFT = @"
                                SELECT  MenuText, MenuAppID, sysMenuID, MenuLevel, sysMenuParent, MenuOrder, ModuleID
                                FROM            sysMenu WITH (NOLOCK)
                                WHERE   (MenuAppID IS NOT NULL)
                                ";
        private const string SQLGetMenuASOFTERPX = @"
                                SELECT      M.sysMenuID, M.MenuName, M.MenuText, M.sysScreenID, M.MenuOrder, M.sysMenuParent, M.CustomerIndex, M.ModuleID, M.MenuLevel, M.ImageUrl ,S.ScreenType 
                                FROM        sysMenu M WITH (NOLOCK)
                                LEFT JOIN   sysScreen S on S.ScreenID = M.sysScreenID
                                WHERE       ISNULL(M.AppOnly, 0) = 0
                                ";

        public async Task<IEnumerable<AppMenu>> GetMenu(CancellationToken cancellationToken = default)
        {
            return await UseConnectionAsync(async connection => await connection.QueryAsync<AppMenu>(SQLGetMenu), cancellationToken);
        }

        public async Task<IEnumerable<AppMenu>> GetMenuASOFT(CancellationToken cancellationToken = default)
        {
            return await UseConnectionAsync(async connection => await connection.QueryAsync<AppMenu>(SQLGetMenuASOFT), cancellationToken);
        }

        /// <summary>
        /// Lấy cấu trúc menu ERPX
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<IEnumerable<ERPXMenu>> GetMenuASOFTERPX(CancellationToken cancellationToken = default)
        {
            return await UseConnectionAsync(async connection => await connection.QueryAsync<ERPXMenu>(SQLGetMenuASOFTERPX), cancellationToken);
        }
    }
}
