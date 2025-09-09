using ASOFT.A00.Entities;
using System;

namespace ASOFT.Notification.Exceptions
{
    /// <summary>
    /// Lỗi token đã được đăng ký với 1 người dùng khác
    /// </summary>
    public class AnotherUserRegisteredDeviceTokenException : Exception
    {
        /// <summary>
        /// Token đã được đăng ký
        /// </summary>
        public AT0012 RegisteredToken { get; }

        /// <summary>
        /// Lỗi token đã được đăng ký với 1 người dùng khác
        /// </summary>
        /// <param name="registeredToken">Token đã được đăng ký</param>
        public AnotherUserRegisteredDeviceTokenException(AT0012 registeredToken) : base(
            $"Token is registered with UserID: {registeredToken?.UserID}")
        {
            RegisteredToken = registeredToken ?? throw new ArgumentNullException(nameof(registeredToken));
        }
    }
}