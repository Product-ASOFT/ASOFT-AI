//####################################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.
//#
//# History:
//#     Date Time       Updater         Comment
//#     22/07/2020      Đoàn Duy        Tạo mới
//####################################################################

using ASOFT.A00.Entities;
using ASOFT.Core.Business.Notification.Entities;
using ASOFT.Notification.Firebase.Requests;
using ASOFT.Notification.Firebase.Respones;
using ASOFT.Notification.Firebase.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Notification.Firebase
{
    public interface IFCMNotificationManager
    {
        /// <summary>
        /// Đăng ký token
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="serviceToken"></param>
        /// <param name="platform"></param>
        /// <param name="clientID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AT0012> SaveDeviceInfoAsync(RegisterDeviceTokenRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Hủy đăng ký token
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="platform"></param>
        /// <param name="clientID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AT0012> UpdateDeviceStatusAsync(UpdateDeviceStatusRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lấy entity token theo service token
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="serviceToken"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task<AT0012> GetDeviceInfo(string serviceToken, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Lấy entity token theo userID và service token
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="serviceToken"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task<AT0012ViewModel> GetDeviceInfo(string userID, string serviceToken, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Gửi message lên Firebase
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<FCMSendRespone> SendNotificaion(OOT9002NotificationRequest request);
    }
}
