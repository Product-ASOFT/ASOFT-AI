using Dapper;
using System;
using System.Data;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Dùng cho Dapper để Dapper có thể  convert những object database type sang C# string type.
    /// </summary>
    public class StringTypeHandler : SqlMapper.TypeHandler<string>
    {
        /// <summary>
        /// String type handler
        /// </summary>
        public static readonly StringTypeHandler Instance = new StringTypeHandler();

        /// <summary>
        /// Parse string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Parse(object value)
        {
            if (value is string str) return str;
            if (null == value || value is DBNull) return null;
            return value.ToString();
        }

        /// <summary>
        /// Set string
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public override void SetValue(IDbDataParameter parameter, string value)
        {
            parameter.Value = value ?? (object) DBNull.Value;
        }
    }
}