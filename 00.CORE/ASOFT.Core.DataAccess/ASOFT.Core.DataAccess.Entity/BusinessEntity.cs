namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Base business entity
    /// </summary>
    public class BusinessEntity : BaseEntity
    {
        /// <summary>
        /// The delete flag. We do not delete record at all. Just set this property for delete and filter this property in database.
        /// </summary>
        public byte? DeleteFlg { get; set; }
    }
}