using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.Core.Business.Common.Entities
{
    public class CRMT90031_REL
    {
        public Guid? APK { get; set; }
        public string DivisionID { get; set; }
        public int? NotesID { get; set; }
        public int? RelatedToTypeID_REL { get; set; }
        public string RelatedToID { get; set; }
    }
}
