using ASOFT.Core.DataAccess;

namespace ASOFT.Core.DataAccess.Relational.Context
{
    /// <summary>
    /// Generic repository cho unit of work pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRelationalContext<T> : IContext<T, IRelationalUnitOfWork>, IRelationalReadonlyContext<T>
        where T : class
    {
    }
}