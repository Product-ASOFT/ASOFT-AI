using System;
using System.Data.SqlTypes;

namespace ASOFT.Core.API.Validation.Attributes
{
    /// <summary>
    /// Validation <see cref="DateTime"/> must be valid <see cref="SqlDateTime"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class SqlDateTimeAttribute : DateTimeRangeAttribute
    {
        /// <inheritdoc />
        public SqlDateTimeAttribute()
            : base(SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessageAccessor">Function return error message</param>
        public SqlDateTimeAttribute(Func<string> errorMessageAccessor) :
            base(SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value, errorMessageAccessor)
        {
        }
    }
}