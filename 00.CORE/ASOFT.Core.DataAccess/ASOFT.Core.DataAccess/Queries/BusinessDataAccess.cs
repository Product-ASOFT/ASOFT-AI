using ASOFT.Core.DataAccess.Relational;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Business data access
    /// </summary>
    public abstract class BusinessDataAccess : RelationalDataAccess
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        protected BusinessDataAccess(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        /// <summary>
        /// Db connection provider key
        /// </summary>
        /// <returns></returns>
        protected override string GetDbConnectionProviderKey() => CommonConnectionKeys.Business;
    }
}