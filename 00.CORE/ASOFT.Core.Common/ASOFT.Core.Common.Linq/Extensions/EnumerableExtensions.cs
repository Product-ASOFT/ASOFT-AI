using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace ASOFT.Linq.Extensions
{
    /// <summary>
    /// An extension class for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Using for custom function class, no need to create new class implements <see cref="System.Collections.Generic.IEqualityComparer{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="equalsFunc"></param>
        /// <param name="hashCodeFunc"></param>
        /// <history>
        /// [Luân Lê] Created 2019/10/01.
        /// </history>
        /// <returns></returns>
        public static IEnumerable<TSource> Union<TSource>([NotNull] this IEnumerable<TSource> first,
            [NotNull] IEnumerable<TSource> second,
            [NotNull] Func<TSource, TSource, bool> equalsFunc,
            [NotNull] Func<TSource, int> hashCodeFunc)
        {
            Checker.NotNull(first, nameof(first));
            Checker.NotNull(second, nameof(second));
            return first.Union(second, new UnionEqualityComparer<TSource>(equalsFunc, hashCodeFunc));
        }

        private class UnionEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equalsFunc;
            private readonly Func<T, int> _hashCodeFunc;

            public UnionEqualityComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashCodeFunc)
            {
                _equalsFunc = Checker.NotNull(equalsFunc, nameof(equalsFunc));
                _hashCodeFunc = Checker.NotNull(hashCodeFunc, nameof(hashCodeFunc));
            }

            public bool Equals(T x, T y) => _equalsFunc(x, y);

            public int GetHashCode(T obj) => _hashCodeFunc(obj);
        }
    }
}