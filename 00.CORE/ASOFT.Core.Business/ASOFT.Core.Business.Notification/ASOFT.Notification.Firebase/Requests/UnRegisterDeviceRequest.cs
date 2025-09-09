namespace ASOFT.Notification.Firebase.Requests
{
    public class UpdateDeviceStatusRequest
    {
        public string UserID { get; set; }
        public string ServiceToken { get; set; }
        public int Status { get; set; }
        public string LastLogInfo { get; set; }
    }
}
