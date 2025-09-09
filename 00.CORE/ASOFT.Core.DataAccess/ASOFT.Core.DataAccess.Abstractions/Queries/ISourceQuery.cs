using System.Linq;

namespace ASOFT.Core.DataAccess
{
    public interface ISourceQuery<T> : IQuery<T>
    {
        IQueryable<T> Query(IQueryable<T> source);
    }
}