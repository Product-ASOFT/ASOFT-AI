using ASOFT.CoreAI.Infrastructure;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class PermissionService : IPermissionHandler
    {
        // xác định loại truy cập dựa trên danh sách quyền
        public Task<string> GetAccessType(List<string> permisions)
        {
            bool hasExternal = permisions.Any(x => x == AccessTypeName.SF2130.ToString());
            bool hasInternal = permisions.Any(x => x == AccessTypeName.SF2140.ToString());
            if (hasInternal && hasExternal)
                return Task.FromResult(AccessTypeName.Both.ToString());

            if (hasInternal)
                return Task.FromResult(AccessTypeName.Internal.ToString());

            if (hasExternal)
                return Task.FromResult(AccessTypeName.External.ToString());

            return Task.FromResult(AccessTypeName.None.ToString());
        }

        // kiểm tra danh sách plugin mà user có quyền truy cập
        public List<string> GetPluginsUserHasAccess(string userId, IEnumerable<string> pluginCodesToCheck)
        {
            return new List<string>
            {
                "OO_AGENT_OOF2110",
                //"CRM_AGENT"
            };
        }
    }
}