using ASOFT.CoreAI.Infrastructure;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class PermissionHandler : IPermissionHandler
    {
        public Task<string> GetAccessType(List<string> permisions)
        {
            bool hasExternal = permisions.Any(x => x == AccessTypeName.SFXXX1.ToString());
            bool hasInternal = permisions.Any(x => x == AccessTypeName.SFXXX2.ToString());
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
            //return _pluginPermissions
            //    .Where(p => p.UserId == userId && p.CanAccess && pluginCodesToCheck.Contains(p.PluginCode))
            //    .Select(p => p.PluginCode)
            //    .Distinct().ToList();
            return new List<string>
            {
                "OO_AGENT_OOF2110",
                //"CRM_AGENT"
            };
        }
    }
}