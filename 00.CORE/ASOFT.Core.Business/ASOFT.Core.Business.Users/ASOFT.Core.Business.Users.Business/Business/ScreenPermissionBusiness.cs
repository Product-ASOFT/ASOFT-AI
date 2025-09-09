using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Users.Business.Interfaces;
using ASOFT.Core.Business.Users.DataAccsess.Interfaces;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Business.Business
{
    public class ScreenPermissionBusiness : IScreenPermissionBusiness
    {
        private readonly IScreenPermissionQueries _screenPermissionQueries;
        public ScreenPermissionBusiness(IScreenPermissionQueries screenPermissionQueries)
        {
            _screenPermissionQueries = screenPermissionQueries;
        }


        /// <summary>
        /// Lấy dữ liệu phân quyền màn hình ERPX
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="divisionID"></param>
        /// <param name="customerIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <history>
        ///     [Minh Nhựt] Tạo mới
        /// </history>
        public async Task<ConcurrentDictionary<string, ASOFTPermission>> GetERPXScreenPermissionAsync(string userID, string divisionID, int customerIndex, CancellationToken cancellationToken = default)
        {
            var permisions = new ConcurrentDictionary<string, ASOFTPermission>();
            var at1403List = await _screenPermissionQueries.GetERPXScreenPermissionAsync(userID, divisionID, customerIndex, cancellationToken);
    
            Parallel.ForEach(at1403List, at1403 =>
            {
                var key = string.Format("{0}_{1}_{2}_{3}",
                        at1403.DivisionID, "", at1403.ModuleID, at1403.ScreenID.Trim()).ToUpper();
                if (!permisions.ContainsKey(key))
                {
                    var permision = ASOFTPermission.None;
                    if (at1403.IsAddNew != 0) permision |= ASOFTPermission.AddNew;
                    if (at1403.IsUpdate != 0) permision |= ASOFTPermission.Update;
                    if (at1403.IsDelete != 0) permision |= ASOFTPermission.Delete;
                    if (at1403.IsPrint != 0) permision |= ASOFTPermission.Print;
                    if (at1403.IsView != 0) permision |= ASOFTPermission.View;
                    if (at1403.IsExportExcel != 0) permision |= ASOFTPermission.IsExportExcel;
                    if (at1403.IsHidden != 0) permision |= ASOFTPermission.Hidden;
                    permisions.TryAdd(key, permision);
                }
            });
            return permisions;
        }
    }
}
