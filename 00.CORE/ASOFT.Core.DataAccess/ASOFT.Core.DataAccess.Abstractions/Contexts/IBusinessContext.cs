using ASOFT.Core.DataAccess.Relational.Context;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Shared relation repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBusinessContext<T> : IBulkRepository<T> where T : class
    {
    }
}