using System;

namespace ASOFT.Notification.Exceptions
{
    /// <summary>
    /// Lỗi được ném ra khi không tìm thấy token của thiết bị.
    /// </summary>
    public class DeviceTokenNotFoundException : Exception
    {
        /// <summary>
        /// Lỗi được ném ra khi không tìm thấy token của thiết bị.
        /// </summary>
        /// <param name="deviceToken"></param>
        public DeviceTokenNotFoundException(string deviceToken) : base($"Device token: '{deviceToken}' is not existed.")
        {
        }

        /// <summary>
        /// Lỗi được ném ra khi không tìm thấy token của thiết bị.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="deviceToken"></param>
        public DeviceTokenNotFoundException(string userID, string deviceToken) : base(
            $"User id: '{userID}' with device token: '{deviceToken}' is not existed.")
        {
        }
    }
}