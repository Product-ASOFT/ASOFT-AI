using Dapper;
using System;
using System.Data;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Dùng để convert <see cref="object"/> sang <see cref="Guid"/> xuống database hoặc từ database lên
    /// <history>
    /// [Luan Le] Created 2019/10/01
    /// </history>
    /// </summary>
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        /// <summary>
        /// Instance class của Guid type handler
        /// </summary>
        public static readonly GuidTypeHandler Instance = new GuidTypeHandler();

        /// <summary>
        /// Set guid
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value;
        }

        /// <summary>
        /// Parse guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Guid Parse(object value)
        {
            switch (value)
            {
                case Guid guid:
                    return guid;
                case string str:
                    return Guid.TryParse(str, out var result) ? result : Guid.Empty;
                default:
                    return default;
            }
        }
    }
}