using Dapper;
using System;
using System.Data;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Dùng để convert <see cref="object"/> sang <see cref="Nullable{Guid}"/> xuống database hoặc từ database lên.
    /// <history>
    /// [Luan Le] Created 2019/10/01
    /// </history>
    /// </summary>
    public class NullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
    {
        /// <summary>
        /// Instance nullable guid type handler
        /// </summary>
        public static readonly NullableGuidTypeHandler Instance = new NullableGuidTypeHandler();

        /// <summary>
        /// Set nullable guid
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public override void SetValue(IDbDataParameter parameter, Guid? value)
        {
            parameter.Value = value ?? (object) DBNull.Value;
        }

        /// <summary>
        /// Parse nullable guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Guid? Parse(object value)
        {
            switch (value)
            {
                case Guid guid:
                    return guid;
                case string str:
                    Guid.TryParse(str, out var result);
                    return result;
                default:
                    return null;
            }
        }
    }
}