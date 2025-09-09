namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Base inheritance entity.
    /// </summary>
    public class InheritanceEntity : BaseEntity
    {
        /// <summary>
        /// The id of inherited voucher.
        /// </summary>
        public string InheritVoucherID { get; set; }

        /// <summary>
        /// The unique id.
        /// </summary>
        public string InheritTransactionID { get; set; }

        /// <summary>
        /// The table id that record inherit
        /// </summary>
        public string InheritTableID { get; set; }
    }
}