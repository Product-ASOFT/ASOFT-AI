namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Base category entity.
    /// </summary>
    public class CategoryEntity : BaseEntity
    {
        /// <summary>
        /// This record in database should be disabled.
        /// </summary>
        public byte? Disabled { get; set; }
    }
}