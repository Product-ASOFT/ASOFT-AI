using ASOFT.Core.DataAccess.Relational.Context;

namespace ASOFT.Core.DataAccess
{
    public interface IAdminContext<T> : IBulkRepository<T> where T : class
    {
    }
}
