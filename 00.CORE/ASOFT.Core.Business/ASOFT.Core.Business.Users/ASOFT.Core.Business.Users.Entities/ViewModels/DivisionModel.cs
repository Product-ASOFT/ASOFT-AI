using ASOFT.Core.DataAccess.Entities;

namespace ASOFT.Core.Business.Users.Entities
{
    /// <summary>
    /// Đối tượng đơn vị
    /// </summary>
    public class DivisionModel : BaseEntity
    {
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public virtual string DivisionName { get; set; }
    }
}