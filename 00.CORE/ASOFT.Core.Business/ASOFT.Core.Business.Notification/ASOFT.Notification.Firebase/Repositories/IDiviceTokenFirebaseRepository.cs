//####################################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.
//#
//# History:
//#     Date Time       Updater         Comment
//#     21/07/2020      Doand Duy        Tạo mới
//####################################################################

using ASOFT.A00.Entities;
using ASOFT.Notification.Firebase.Requests;
using System.Threading.Tasks;

namespace ASOFT.Notification.Domain.Aggregates
{
    /// <summary>
    /// Repository cho việc truy cập dữ liệu của token thiết bị.
    /// </summary>
    public interface IDiviceTokenFirebaseRepository
    {
        /// <summary>
        /// Repository cho việc truy cập dữ liệu của token thiết bị.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="excludeDeviceToken"></param>
        /// <returns></returns>
        //Task<IEnumerable<AT0012>> GetListDeviceTokenByUser(string userID, IEnumerable<string> excludeDeviceToken);

        /// <summary>
        /// Thêm mới token
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Add(AT0012 entity);

       /// <summary>
       /// 
       /// </summary>
       /// <param name="entity"></param>
       /// <param name="apk"></param>
       /// <returns></returns>
        Task<int> UpdateDeviceInfo(RegisterDeviceTokenRequest request, AT0012 entity);

        /// <summary>
        /// Xóa token
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> RemoveNotifyToken(AT0012 entity);
    }
}
