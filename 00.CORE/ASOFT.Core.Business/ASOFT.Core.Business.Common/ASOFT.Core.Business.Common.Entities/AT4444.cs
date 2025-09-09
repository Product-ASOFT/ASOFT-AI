using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ASOFT.Core.Business.Common.Entities
{
    /// <summary>
    /// Sinh mã bút toán cho các PK.
    /// </summary>
    public class AT4444
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid APK { get; set; }

        public string DivisionID { get; set; }
        public string TABLENAME { get; set; }
        public string KEYSTRING { get; set; }
        public int? LASTKEY { get; set; }

        // [Tấn Thành] - [17/12/2020] - BEGIN ADD
        public const string COL_APK = "APK";
        public const string COL_DIVISIONID = "DivisionID";
        public const string COL_TABLENAME = "TABLENAME";
        public const string COL_KEYSTRING = "KEYSTRING";
        public const string COL_LASTKEY = "LASTKEY";
        // [Tấn Thành] - [17/12/2020] - END ADD
    }
}