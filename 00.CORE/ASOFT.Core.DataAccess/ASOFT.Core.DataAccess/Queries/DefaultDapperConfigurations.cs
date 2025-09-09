using ASOFT.Core.DataAccess;
using Dapper;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Thiết lập cho Dapper
    /// </summary>
    public static class DefaultDapperConfigurations
    {
        /// <summary>
        /// Thiết lập mặc định cho Dapper
        /// </summary>
        public static void Configure()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(StringTypeHandler.Instance);
            SqlMapper.AddTypeHandler(GuidTypeHandler.Instance);
            SqlMapper.AddTypeHandler(NullableGuidTypeHandler.Instance);
        }
    }
}