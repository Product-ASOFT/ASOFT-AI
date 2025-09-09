namespace ASOFT.Core.Business.Devices.Business.Queries.ViewModels
{
    public class ClientVersionViewModel
    {
        public string ClientID { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public int VersionCode { get; set; }
        public int Level { get; set; }
        public string LevelName { get; set; }
        public string ClientName { get; set; }
        public string DownloadLink { get; set; }
        public string Message { get; set; }
    }
}