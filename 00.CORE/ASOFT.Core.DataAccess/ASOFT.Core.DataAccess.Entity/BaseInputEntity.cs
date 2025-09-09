namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Using for input parameters of function, class...
    /// </summary>
    public class BaseInputEntity
    {
        /// <summary>
        /// In database table of ASOFT. Always has <see cref="DivisionID"/>.
        /// </summary>
        public virtual string DivisionID { get; set; }

        /// <summary>
        /// Current user id.
        /// </summary>
        public virtual string UserID { get; set; }
    }
}