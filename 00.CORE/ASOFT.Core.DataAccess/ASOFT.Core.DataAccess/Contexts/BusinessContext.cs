namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Generic shared repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusinessContext<T> : EFGenericContext<T, BusinessDbContext>, IBusinessContext<T>
        where T : class
    {
        /// <summary>
        /// Generic shared repository
        /// </summary>
        /// <param name="context"></param>
        public BusinessContext(BusinessDbContext context) : base(context)
        {
        }
    }
}