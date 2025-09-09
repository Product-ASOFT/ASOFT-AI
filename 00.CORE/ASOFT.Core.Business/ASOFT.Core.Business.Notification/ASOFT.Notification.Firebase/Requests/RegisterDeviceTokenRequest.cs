using System;

namespace ASOFT.Notification.Firebase.Requests
{
    public class RegisterDeviceTokenRequest
    {
        public string DivisionID { get; set; }
        public string UserID { get; set; }
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceBrand { get; set; }
        public string DeviceOSVersion { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceToken { get; set; }
        public string NotifyToken { get; set; }
        public DateTime? FirstInstallTime { get; set; }
        public DateTime? LastedUpdateTime { get; set; }
        public string AppVersion { get; set; }
        public string LastIPAddress { get; set; }
        public DateTime? LastedLoginTime { get; set; }
        public string LastLogInfo { get; set; }
        public string LastLogWarn { get; set; }
        public string LastLogError { get; set; }
        public byte? Disable { get; set; }
        public int? Status { get; set; }
    }
}
