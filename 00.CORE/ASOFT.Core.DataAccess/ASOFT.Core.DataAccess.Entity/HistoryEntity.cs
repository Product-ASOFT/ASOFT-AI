using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.Core.DataAccess.Entities
{
    public class HistoryEntity
    {
        public Guid? APK { set; get; }
        public string DivisionID { set; get; }
        public int? HistoryID { set; get; }
        public string Description { set; get; }
        public string RelatedToID { set; get; }
        public int? RelatedToTypeID { set; get; }
        public DateTime? CreateDate { set; get; }
        public string CreateUserID { set; get; }
        public int? StatusID { set; get; }
        public string ScreenID { set; get; }
        public string TableID { set; get; }

    }
}
