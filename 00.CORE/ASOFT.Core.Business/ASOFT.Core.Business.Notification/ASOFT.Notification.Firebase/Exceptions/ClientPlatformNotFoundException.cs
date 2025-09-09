using System;

namespace ASOFT.Notification.Exceptions
{
    /// <summary>
    /// Lỗi không tìm thấy nền tảng
    /// </summary>
    public class ClientPlatformNotFoundException : Exception
    {
        /// <summary>
        /// Id của client gửi lên trên server
        /// </summary>
        public string ClientID { get; }

        /// <summary>
        /// Nền tảng dưới client gửi lên
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// Lỗi không tìm thấy nền tảng
        /// </summary>
        /// <param name="clientID">Id của client gửi lên trên server</param>
        /// <param name="platform">Nền tảng dưới client gửi lên</param>
        public ClientPlatformNotFoundException(string clientID, string platform) : base(
            $"Platform : {platform} with clientID: {clientID} is not found.")
        {
            ClientID = clientID;
            Platform = platform;
        }
    }
}