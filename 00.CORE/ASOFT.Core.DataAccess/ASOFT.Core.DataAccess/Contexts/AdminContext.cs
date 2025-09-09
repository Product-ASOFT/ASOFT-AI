

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Admin repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AdminContext<T>: EFGenericContext<T, AdminDbContext>, IAdminContext<T> where T: class
    {
        /// <summary>
        /// Admin Repository
        /// </summary>
        /// <param name="context"></param>
        public AdminContext(AdminDbContext context) : base(context)
        {
        }
    }
}
