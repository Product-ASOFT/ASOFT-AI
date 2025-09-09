//####################################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.
//#
//# History:
//#     Date Time       Updater         Comment
//#     21/07/2020      Đoàn Duy        Tạo mới
//####################################################################

using ASOFT.A00.Entities;
using ASOFT.Core.Business.Notification.Entities;
using ASOFT.Notification.Exceptions;
using ASOFT.Notification.Firebase.Repositories;
using ASOFT.Notification.Firebase.Requests;
using ASOFT.Notification.Firebase.Respones;
using ASOFT.Notification.Firebase.ViewModels;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Notification.Firebase
{
    /// <summary>
    /// Class quản lý FCM
    /// </summary>
    public class FCMNotificationManager : IFCMNotificationManager
    {
        private readonly DiviceTokenFirebaseRepository _deviceTokenRepository;
        public FCMNotificationManager(string connectionString)
        {
            _deviceTokenRepository = new DiviceTokenFirebaseRepository(connectionString);

            //Khởi tạo Firebase app với config được lấy theo biến môi trường của window    
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault(),

                });
            }
        }

        /// <summary>
        /// Lấy entity token theo service token
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="serviceToken"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<AT0012> GetDeviceInfo(string serviceToken, CancellationToken cancellation = default(CancellationToken))
        {
            return await _deviceTokenRepository.GetDeviceInfo(serviceToken);
        }

        /// <summary>
        /// Lấy entity token theo userID và service token
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="deviceOS"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<AT0012ViewModel> GetDeviceInfo(string deviceID, string deviceOS, CancellationToken cancellation = default(CancellationToken))
        {
            return await _deviceTokenRepository.GetDeviceInfo(deviceID, deviceOS);
        }

        /// <summary>
        /// Đăng ký token và thông tin thiết bị
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AT0012> SaveDeviceInfoAsync(RegisterDeviceTokenRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Lấy đối tượng token
                var tokenEntity = await GetDeviceInfo(request.DeviceID, request.DeviceOS, cancellationToken);
                var createEntityNeed = null == tokenEntity;

                // Nếu cần phải khởi tạo đối tượng token
                if (createEntityNeed)
                {
                    tokenEntity = new AT0012ViewModel
                    {
                        APK = Guid.NewGuid(),
                        UserID = request.UserID,
                        NotifyToken = request.NotifyToken,
                        AppVersion = request.AppVersion,
                        DeviceID = request.DeviceID,
                        DeviceName = request.DeviceName,
                        DeviceBrand = request.DeviceBrand,
                        DeviceOSVersion = request.DeviceOSVersion,
                        DeviceOS = request.DeviceOS,
                        DeviceToken = request.DeviceToken,
                        DivisionID = request.DivisionID,
                        FirstInstallTime = request.FirstInstallTime,
                        LastedUpdateTime = request.LastedUpdateTime,
                        LastIPAddress = request.LastIPAddress,
                        LastLogError = request.LastLogError,
                        LastLogInfo = request.LastLogInfo,
                        LastLogWarn = request.LastLogWarn,
                        Status = 0,
                        Disable = 0,
                        CreateUserID = request.UserID,
                        LastModifyUserID = request.UserID,
                    };
                    var utcNow = DateTime.Now;
                    tokenEntity.CreateDate = utcNow;
                    tokenEntity.LastModifyDate = utcNow;
                    tokenEntity.LastedLoginTime = utcNow;

                    await _deviceTokenRepository.Add(tokenEntity);
                }
                else
                {
                    //Trường hợp thiết bị bị khóa 
                    if (tokenEntity.Status == 2 && tokenEntity.DivisionID == request.DivisionID)
                    {
                        //Trường hợp mở khóa user
                        if (tokenEntity.IsLock == 0 || tokenEntity.UserID != request.UserID)
                        {
                            tokenEntity.Status = request.Status ?? 0;
                            await _deviceTokenRepository.UpdateDeviceInfo(request, tokenEntity);
                        }

                        return tokenEntity;
                    }

                    //Trường hợp tài khoản bị khóa thì disable toàn bộ thiết bị đã đăng nhập
                    if (tokenEntity.IsLock == 1)
                    {
                        if(tokenEntity.UserID != request.UserID)
                        {
                            tokenEntity.Status = request.Status ?? 0;
                            tokenEntity.UserID = request.UserID;
                            await _deviceTokenRepository.UpdateDeviceInfo(request, tokenEntity);
                        }
                        else if (tokenEntity.Status != 2)
                        {
                            tokenEntity.Status = 2;
                            await _deviceTokenRepository.UpdateDevicesStatusByUser(tokenEntity.UserID, 2);
                        }

                        return tokenEntity;
                    }

                    //Cập nhật thông tin khi sử dụng app thiết bị
                    if (request.Status == 0)
                    {
                        request.LastedLoginTime = DateTime.Now;
                    }
                    await _deviceTokenRepository.UpdateDeviceInfo(request, tokenEntity);
                }

                return tokenEntity;
            }
            catch (Exception e)
            {

                throw e;
            }
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AT0012> UpdateDeviceStatusAsync(UpdateDeviceStatusRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Lấy đối tượng token
            var tokenEntity = await GetDeviceInfo(request.ServiceToken, cancellationToken);

            // Nếu không tìm thấy token
            if (tokenEntity == null)
            {
                throw new DeviceTokenNotFoundException(request.UserID, request.ServiceToken);
            }
            tokenEntity.Status = request.Status;
            tokenEntity.LastLogInfo = request.LastLogInfo;
            await _deviceTokenRepository.RemoveNotifyToken(tokenEntity);

            return tokenEntity;
        }

        /// <summary>
        /// Tách mảng lớn thành các mảng nhỏ hơn
        /// </summary>
        /// <param name="orginList"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static List<List<string>> SplitList(List<string> orginList, int size)
        {
            var list = new List<List<string>>();

            for (int i = 0; i < orginList.Count; i += size)
            {
                list.Add(orginList.GetRange(i, Math.Min(size, orginList.Count - i)));
            }

            return list;
        }

        /// <summary>
        /// Gửi message lên Firebase
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<FCMSendRespone> SendNotificaion(OOT9002NotificationRequest request)
        {
            //Danh sách token của người dùng
            var registrationTokens = await _deviceTokenRepository.GetNotifyTokensByUser(request.UserIDList);

            //Kết quả trả về
            var result = new FCMSendRespone { FailureCount = 0, SuccessCount = 0 };

            if (registrationTokens.Count == 0)
            {
                return result;
            }

            //Danh sách token không hợp lệ
            var invalidTokenList = new List<string>();

            var badges = new ConcurrentBag<ParallelRespone>();

            //Kiểm tra nếu title và body được truyền vào có trùng hay ko
            string body = string.Empty;
            if (request.Title != request.Body)
            {
                body = request.Body;
            }
            //Tạo nội dung cho message gửi đi
            var notification = new FirebaseAdmin.Messaging.Notification
            {
                Body = body,
                Title = request.Title,
                ImageUrl = request.ImageURL,
            };
            //Data cho message
            var data = new Dictionary<string, string>()
                {
                    { "apk", request.APK.ToString() },
                    { "type", request.Type },
                    { "createUserID", request.CreateUserID },
                    { "messageAPK", request.MessageAPK.ToString() },
                    { "extraData1", request.ExtraData1 },
                    { "extraData2", request.ExtraData2 },
                    { "extraData3", request.ExtraData3 },
                };
            //Config cho ios
            var apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Sound = "default",
                    //MutableContent = true,
                    //ContentAvailable = true,
                    
                },
                Headers = new Dictionary<string, string>() {
                    { "mutable-content", "1" }
                },

                FcmOptions = new ApnsFcmOptions
                {
                    ImageUrl = request.ImageURL
                },
            };
            //Config cho android
            var android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    Sound = "default",
                    ImageUrl = request.ImageURL,
                    Visibility = NotificationVisibility.PUBLIC,
                    Priority = NotificationPriority.HIGH
                },
                Priority = Priority.High,
            };

            var message = new Message
            {
                Android = android,
                Apns = apns,
                Notification = notification,
                Data = data,
            };

            foreach (var item in registrationTokens)
            {
                try
                {
                    message.Token = item.NotifyToken;
                    message.Apns.Aps.Badge = item.UnReadChat + item.UnReadNoti;
                    string resultMessage = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
                    if(resultMessage != null)
                    {
                        result.SuccessCount++;
                    }
                    else
                    {
                        invalidTokenList.Add(item.NotifyToken);
                        result.FailureCount++;
                    }
                }
                catch (Exception e)
                {
                    invalidTokenList.Add(item.NotifyToken);
                    result.FailureCount++;
                    continue;
                }
            }

            return result;
        }

        /// <summary>
        /// Gửi message lên Firebase
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<FCMSendRespone> SendNotificaionByToken(OOT9002NotificationRequest request)
        {

            //Kết quả trả về
            var result = new FCMSendRespone { FailureCount = 0, SuccessCount = 0 };

            if (request.RegistrationTokens.Count == 0)
            {
                return result;
            }

            //Danh sách token không hợp lệ
            var invalidTokenList = new List<string>();

            var badges = new ConcurrentBag<ParallelRespone>();

            //Kiểm tra nếu title và body được truyền vào có trùng hay ko
            string body = string.Empty;
            if (request.Title != request.Body)
            {
                body = request.Body;
            }
            //Tạo nội dung cho message gửi đi
            var notification = new FirebaseAdmin.Messaging.Notification
            {
                Body = body,
                Title = request.Title,
                ImageUrl = request.ImageURL,

            };
            //Data cho message
            var data = new Dictionary<string, string>()
                {
                    { "apk", request.APK.ToString() },
                    { "type", request.Type },
                    { "createUserID", request.CreateUserID },
                    { "messageAPK", request.MessageAPK.ToString() },
                    { "extraData1", request.ExtraData1 },
                    { "extraData2", request.ExtraData2 },
                    { "extraData3", request.ExtraData3 },
                };
            //Config cho ios
            var apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Sound = "default",
                    //MutableContent = true,
                    //ContentAvailable = true,

                },
                Headers = new Dictionary<string, string>() {
                    { "mutable-content", "1" }
                },

                FcmOptions = new ApnsFcmOptions
                {
                    ImageUrl = request.ImageURL
                },
            };
            //Config cho android
            var android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    Sound = "default",
                    ImageUrl = request.ImageURL,
                },
                Priority = Priority.High
            };

            var message = new Message
            {
                Android = android,
                Apns = apns,
                Notification = notification,
                Data = data,
            };

            foreach (var item in request.RegistrationTokens)
            {
                try
                {
                    message.Token = item.NotifyToken;
                    message.Apns.Aps.Badge = item.UnReadChat + item.UnReadNoti;
                    await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    result.SuccessCount++;
                }
                catch (Exception e)
                {
                    invalidTokenList.Add(item.NotifyToken);
                    result.FailureCount++;
                    continue;
                }
            }

            return result;
        }

        /// <summary>
        /// Gửi data message lên Firebase
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<FCMSendRespone> SendDataMessage(OOT9002NotificationRequest request)
        {
            //Danh sách token của người dùng
            var registrationTokens = await _deviceTokenRepository.GetNotifyTokensPlatforms(request.UserIDList, request.PlatformList);

            //Kết quả trả về
            var result = new FCMSendRespone { FailureCount = 0, SuccessCount = 0 };

            if (registrationTokens.Count == 0)
            {
                return result;
            }

            //Danh sách token không hợp lệ
            var invalidTokenList = new List<string>();

            var badges = new ConcurrentBag<ParallelRespone>();

            //Data cho message
            var data = new Dictionary<string, string>()
                {
                    { "commandType", request.CommandType },
                };
            //Config cho ios
            var apns = new ApnsConfig
            {

            };
            //Config cho android
            var android = new AndroidConfig
            {

            };
            //Nếu số lượng token > 100
            //Firebase giới hạn mỗi request chỉ được 100 token
            if (registrationTokens.Count > 100)
            {
                //Tách mảng token thành các mảng nhỏ 100 phần tử
                var tokensSplitedList = SplitList(registrationTokens, 100);

                async Task TokenListTask(List<string> tokenList)
                {
                    var message = new MulticastMessage()
                    {
                        Data = data,
                        Tokens = tokenList,
                        //Android = android,
                        //Apns = apns
                    };
                    var fcmResult = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

                    badges.Add(new ParallelRespone { TokenList = tokenList, FcmRespones = fcmResult });
                }

                await tokensSplitedList.ParallelForEachAsync(TokenListTask);

                foreach (var badge in badges)
                {
                    result.SuccessCount += badge.FcmRespones.SuccessCount;
                    result.FailureCount += badge.FcmRespones.FailureCount;
                    //Nếu có message gửi thất bại
                    if (badge.FcmRespones.FailureCount > 0)
                    {
                        int i = 0;
                        foreach (var item in badge.FcmRespones.Responses)
                        {
                            //Nếu respone trả về fail 
                            if (!item.IsSuccess)
                            {
                                //Trường hợp FCM refesh token hoặc app của người dùng không còn đăng ký FCM
                                if (item.Exception.MessagingErrorCode == MessagingErrorCode.Unregistered)
                                {
                                    invalidTokenList.Add(badge.TokenList[i]);
                                }
                                //Trường hợp FCM token lưu hợp lệ
                                if (badge.FcmRespones.SuccessCount > 0 && item.Exception.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                                {
                                    invalidTokenList.Add(badge.TokenList[i]);
                                }
                            }
                            i++;
                        }
                    }
                }
                await _deviceTokenRepository.RemoveBulkNotifyToken(invalidTokenList);
            }
            else
            {
                var message = new MulticastMessage()
                {
                    Data = data,
                    Tokens = registrationTokens,
                    // Android = android,
                    //Apns = apns
                };

                var fcmResult = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

                result.FailureCount = fcmResult.FailureCount;
                result.SuccessCount = fcmResult.SuccessCount;

                //Nếu có message gửi thất bại
                if (fcmResult.FailureCount > 0)
                {
                    int i = 0;
                    foreach (var item in fcmResult.Responses)
                    {
                        //Nếu respone trả về fail 
                        if (!item.IsSuccess)
                        {
                            //Trường hợp FCM refesh token hoặc app của người dùng không còn đăng ký FCM
                            if (item.Exception.MessagingErrorCode == MessagingErrorCode.Unregistered)
                            {
                                invalidTokenList.Add(registrationTokens[i]);
                            }
                            //Trường hợp FCM token lưu hợp lệ
                            if (fcmResult.SuccessCount > 0 && item.Exception.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                            {
                                invalidTokenList.Add(registrationTokens[i]);
                            }
                        }
                        i++;
                    }
                    await _deviceTokenRepository.RemoveBulkNotifyToken(invalidTokenList);
                }
            }
            return result;
        }
    }

}
