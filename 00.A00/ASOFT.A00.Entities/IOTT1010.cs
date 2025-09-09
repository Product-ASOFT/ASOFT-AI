using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.A00.Entities
{
    public class IOTT1010:BaseEntity
    {
        public string ClientDomain { get; set; }
        public string RefeshToken { get; set; }
        public string IOTHost { get; set; }
        public byte? IsRenew { get; set; }
        public byte? Disabled { get; set; }
        public byte? DeleteFlg { get; set; }
    }
}
