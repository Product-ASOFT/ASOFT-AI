using System;
using System.Collections.Generic;

namespace ASOFT.Core.Business.Files.Entities.Requests
{
    public class RemoveFileRequest 
    {
        public string DivisionID { get; set; }
        public List<Guid> APKs { get; set; }
        public string UserID { get; set; }
        public string ScreenID { get; set; }
        public string TableID { get; set; }
        public Guid APKMaster { get; set; }
    }
}
