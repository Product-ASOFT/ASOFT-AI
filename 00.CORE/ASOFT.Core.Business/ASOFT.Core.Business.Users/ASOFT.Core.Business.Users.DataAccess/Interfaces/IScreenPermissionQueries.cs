// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    20/08/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Users.Entities.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.DataAccsess.Interfaces
{
    public interface IScreenPermissionQueries
    {
        /// <summary>
        /// Lấy dữ liệu phân quyền màn hình APP
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="DivisionID"></param>
        /// <returns></returns>
        Task<IEnumerable<AP1403ViewModel>> GetScreenPermissionAsync(string userID, string DivisionID, CancellationToken cancellationToken);

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
        Task<IEnumerable<AP1403ViewModel>> GetERPXScreenPermissionAsync(string userID, string divisionID, int customerIndex, CancellationToken cancellationToken);

        /// <summary>
        /// Lấy dữ  liệu phân quyền màn hình cho khách hàng dùng app "module CCM"
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="DivisionID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //private const string SQL_GetCustomerPermisson = @"select * from AT1403 where GroupID = 'ccm_permisson_groupid' and ModuleID = 'ASOFTCCM' and DivisionID=@DivisionID";
        Task<IEnumerable<AP1403ViewModel>> GetCustomerPermisson(string groupID, string DivisionID, CancellationToken cancellationToken);
    }
}
