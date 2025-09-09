using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Base entity for database table of ASOFT
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// The unique identify value of table.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid APK { get; set; }

        /// <summary>
        /// In database table of ASOFT. Always has <see cref="DivisionID"/>.
        /// </summary>
        public string DivisionID { get; set; }

        /// <summary>
        /// The id of user that creates record.
        /// </summary>
        public string CreateUserID { set; get; }

        /// <summary>
        /// The date time when record is created.
        /// </summary>
        public DateTime? CreateDate { set; get; }

        /// <summary>
        /// The id of lasted user that modified record.
        /// </summary>
        public string LastModifyUserID { set; get; }

        /// <summary>
        /// The date time when record is lasted modified.
        /// </summary>
        public DateTime? LastModifyDate { set; get; }
    }
}