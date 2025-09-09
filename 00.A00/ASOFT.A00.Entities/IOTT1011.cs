using System;

namespace ASOFT.A00.Entities
{
    public class IOTT1011
    {
        public Guid? APK { get; set; }
        public string AccessToken { get; set; }
        public string ClientDomain { get; set; }
        public string IOTHost { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
