using ASOFT.Core.DataAccess;

namespace ASOFT.Core.DataAccess.Relational.Context
{
    /// <summary>
    /// Relational readonly repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRelationalReadonlyContext<T> : IReadonlyContext<T, IRelationalUnitOfWork> where T : class
    {
    }
}