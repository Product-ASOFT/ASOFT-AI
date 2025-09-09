using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Business.Users.DataAccsess.Interfaces;
using ASOFT.Core.Business.Users.Entities.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Business
{
    public class MenuBusiness: IMenuBusiness
    {
        private readonly IMenuQueries _menuQueries;
        private readonly IScreenPermissionQueries _screenPermissionQueries;

        public MenuBusiness(IMenuQueries menuQueries, IScreenPermissionQueries screenPermissionQueries)
        {
            _menuQueries = Checker.NotNull(menuQueries, nameof(menuQueries));
            _screenPermissionQueries = Checker.NotNull(screenPermissionQueries, nameof(screenPermissionQueries));
        }


        public async Task<Dictionary<string, AppMenu>> GetMenu(string userID, string divisionID, IEnumerable<AP1403ViewModel> permissions, CancellationToken cancellationToken = default)
        {
            var data = await _menuQueries.GetMenu(cancellationToken);
            var level0 = data.Where(m => m.MenuLevel == 0).OrderBy(m => m.MenuOrder).ToDictionary(m => m.MenuAppID.ToUpper(), m => m);

            foreach (var module in level0)
            {
                module.Value.Children = data.Where(m => m.ModuleID.ToUpper() == module.Value.ModuleID.ToUpper() && m.MenuLevel == 2).OrderBy(m => m.MenuOrder).ToList();
                var isViewLv0 = permissions.FirstOrDefault(m => m.ScreenID.ToUpper() == module.Value.ModuleID.ToUpper());
                try
                {
                    module.Value.IsView = isViewLv0.IsView;
                }
                catch (System.Exception e)
                {
                    module.Value.IsView = 0;
                }
                if (module.Value.Children.Count > 0 && module.Value.Children != null)
                    foreach (var group in module.Value.Children)
                    {
                        group.Children = data.Where(m => m.sysMenuParent == group.sysMenuID).OrderBy(m => m.MenuOrder).ToList();
                        foreach (var screen in group.Children)
                        {
                            try
                            {
                                var permission = permissions.FirstOrDefault(m => m.ScreenID.ToUpper() == screen.MenuAppID.ToUpper());
                                screen.IsView = permission != null ? permission.IsView : (byte)0;
                                screen.IsHidden = permission != null ? permission.IsHidden : (byte)0;
                            }
                            catch (System.Exception e)
                            {
                                screen.IsView = 0;
                                screen.IsHidden = 0;
                            }
                        }
                    }
            }

            return level0;
        }

        public async Task<Dictionary<string, AppMenu>> GetMenuASOFT(string userID, string divisionID, IEnumerable<AP1403ViewModel> permissions, CancellationToken cancellationToken = default)
        {
            var data = await _menuQueries.GetMenuASOFT(cancellationToken);
            var level0 = data.Where(m => m.MenuLevel == 0).OrderBy(m => m.MenuOrder).ToDictionary(m => m.MenuAppID.ToUpper(), m => m);

            foreach (var module in level0)
            {
                module.Value.Children = data.Where(m => m.ModuleID.ToUpper() == module.Value.ModuleID.ToUpper() && (m.MenuLevel == 2 || m.MenuLevel == 1)).OrderBy(m => m.MenuOrder).ToList();
                var isViewLv0 = permissions.FirstOrDefault(m => m.ScreenID.ToUpper() == module.Value.ModuleID.ToUpper());
                try
                {
                    module.Value.IsView = isViewLv0.IsView;
                }
                catch (System.Exception e)
                {
                    module.Value.IsView = 0;
                }
                if (module.Value.Children.Count > 0 && module.Value.Children != null)
                    foreach (var group in module.Value.Children)
                    {
                        group.Children = data.Where(m => m.sysMenuParent == group.sysMenuID).OrderBy(m => m.MenuOrder).ToList();
                        foreach (var screen in group.Children)
                        {
                            try
                            {
                                var permission = permissions.FirstOrDefault(m => m.ScreenID.ToUpper() == screen.MenuAppID.ToUpper());
                                screen.IsView = permission != null ? permission.IsView : (byte)0;
                                screen.IsHidden = permission != null ? permission.IsHidden : (byte)0;
                            }
                            catch (System.Exception e)
                            {
                                screen.IsView = 0;
                                screen.IsHidden = 0;
                            }
                        }
                    }
            }

            return level0;
        }

        public async Task<Dictionary<string, AppMenu>> GetMenu(CancellationToken cancellationToken = default)
        {
            var data = await _menuQueries.GetMenu(cancellationToken);
            var level0 = data.Where(m => m.MenuLevel == 0).OrderBy(m => m.MenuOrder).ToDictionary(m => m.MenuAppID.ToUpper(), m => m);

            foreach (var module in level0)
            {
                module.Value.Children = data.Where(m => m.ModuleID.ToUpper() == module.Value.ModuleID.ToUpper() && m.MenuLevel == 2).OrderBy(m => m.MenuOrder).ToList();
                if (module.Value.Children.Count > 0 && module.Value.Children != null)
                    foreach (var group in module.Value.Children)
                    {
                        group.Children = data.Where(m => m.sysMenuParent == group.sysMenuID).OrderBy(m => m.MenuOrder).ToList();                      
                    }
            }

            return level0;
        }


        /// <summary>
        /// Lấy cấu trúc menu 1BOSS từ DB admin
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, AppMenu>> GetMenu(string userID, string divisionID, CancellationToken cancellationToken = default)
        {
            var data = await _menuQueries.GetMenu(cancellationToken);
            var permissions = await _screenPermissionQueries.GetScreenPermissionAsync(userID, divisionID, cancellationToken);
            var level0 = data.Where(m => m.MenuLevel == 0).OrderBy(m => m.MenuOrder).ToDictionary(m => m.MenuAppID.ToUpper(), m => m);

            foreach (var module in level0)
            {
                module.Value.Children = data.Where(m => m.ModuleID.ToUpper() == module.Value.ModuleID.ToUpper() && m.MenuLevel == 2).OrderBy(m => m.MenuOrder).ToList();
                var isViewLv0 = permissions.FirstOrDefault(m => m.ScreenID.ToUpper() == module.Value.ModuleID.ToUpper());
                module.Value.IsView = isViewLv0.IsView;
                if (module.Value.Children.Count > 0 && module.Value.Children != null)
                    foreach (var group in module.Value.Children)
                    {
                        group.Children = data.Where(m => m.sysMenuParent == group.sysMenuID).OrderBy(m => m.MenuOrder).ToList();
                        foreach (var screen in group.Children)
                        {
                            var permission = permissions.FirstOrDefault(m => m.ScreenID.ToUpper() == screen.MenuAppID.ToUpper());
                            screen.IsView = permission != null ? permission.IsView : byte.Parse("0");
                        }
                    }
            }

            return level0;
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
            var result = await _menuQueries.GetMenuASOFTERPX(cancellationToken);
            Parallel.ForEach(result, item =>
            {
                if (!string.IsNullOrEmpty(item.MenuText) && item.MenuText.Contains("A00."))
                {
                    item.MenuText = item.MenuText.Replace("A00.", string.Empty);
                }
            });

            return result;
        }
    }
}
