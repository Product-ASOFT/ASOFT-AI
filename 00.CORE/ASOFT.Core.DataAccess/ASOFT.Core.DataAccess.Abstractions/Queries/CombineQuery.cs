using System;
using System.Collections.Generic;
using System.Linq;

namespace ASOFT.Core.DataAccess
{
    public class CombineQuery<T> : SourceQuery<T>, ICombineQuery<T>
    {
        private readonly ICollection<ISourceQuery<T>> _sourceSpecs;

        public CombineQuery(IEnumerable<ISourceQuery<T>> specs) => _sourceSpecs =
            new List<ISourceQuery<T>>(specs ?? throw new ArgumentNullException(nameof(specs)));

        public CombineQuery(params ISourceQuery<T>[] specs) => _sourceSpecs =
            new List<ISourceQuery<T>>(specs ?? throw new ArgumentNullException(nameof(specs)));

        public CombineQuery(string id, IEnumerable<ISourceQuery<T>> specs) : base(id)
            => _sourceSpecs =
                new List<ISourceQuery<T>>(specs ?? throw new ArgumentNullException(nameof(specs)));

        public CombineQuery(string id, params ISourceQuery<T>[] specs) : base(id)
            => _sourceSpecs =
                new List<ISourceQuery<T>>(specs ?? throw new ArgumentNullException(nameof(specs)));

        public virtual void Add(ISourceQuery<T> spec) =>
            _sourceSpecs.Add(spec ?? throw new ArgumentNullException(nameof(spec)));

        protected override IQueryable<T> GetQuery(IQueryable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_sourceSpecs.Any())
            {
                return _sourceSpecs.Aggregate(source, (src, spec) => spec.Query(src));
            }

            return source;
        }
    }
}