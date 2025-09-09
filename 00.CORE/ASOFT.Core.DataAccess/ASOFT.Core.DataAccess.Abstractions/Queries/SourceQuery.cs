using System;
using System.Linq;

namespace ASOFT.Core.DataAccess
{
    public abstract class SourceQuery<T> : BaseQuery<T>, ISourceQuery<T>
    {
        protected SourceQuery()
        {
        }

        protected SourceQuery(string id) : base(id)
        {
        }

        public IQueryable<T> Query(IQueryable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return GetQuery(source);
        }

        protected abstract IQueryable<T> GetQuery(IQueryable<T> source);

        private class FuncSourceSpecification : SourceQuery<T>
        {
            private readonly Func<IQueryable<T>, IQueryable<T>> _funcSpec;

            public FuncSourceSpecification(Func<IQueryable<T>, IQueryable<T>> funcSpec) => _funcSpec = funcSpec;

            protected override IQueryable<T> GetQuery(IQueryable<T> source) => _funcSpec(source);
        }
    }
}