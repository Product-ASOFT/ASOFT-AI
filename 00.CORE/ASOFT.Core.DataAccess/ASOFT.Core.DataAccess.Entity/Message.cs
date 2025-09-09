using System;

namespace ASOFT.Core.DataAccess.Entites
{
    public class Message
    {
        public string ID { get; set; }
        public string LanguageID { get; set; }
        public string Name { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Module { get; set; }
        public bool? Deleted { get; set; }
        public string CustomName { get; set; }
        public string FormID { get; set; }
    }
}