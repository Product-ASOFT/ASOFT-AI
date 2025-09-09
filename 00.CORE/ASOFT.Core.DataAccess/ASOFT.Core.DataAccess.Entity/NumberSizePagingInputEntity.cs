namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// The paging entity.
    /// </summary>
    public class NumberSizePagingInputEntity : BaseInputEntity
    {
        /// <summary>
        /// The number of page to request.
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// The size of page to request.
        /// </summary>
        public int? PageSize { get; set; }
    }
}