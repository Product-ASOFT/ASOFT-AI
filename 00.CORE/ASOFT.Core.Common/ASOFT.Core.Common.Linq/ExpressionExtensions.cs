using ASOFT.Core.Common.InjectionChecker;
using System;
using System.Linq.Expressions;

namespace ASOFT.Core.Common.Linq
{
    /// <summary>
    /// An expression extension class for <see cref="Expression"/> or <see cref="Expression{TDelegate}"/>
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Expression for not body.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <history>
        /// [Luan Le] Created 2019/10/07
        /// </history>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            Checker.NotNull(expression, nameof(expression));
            Expression notBody = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(notBody, expression.Parameters);
        }
    }
}