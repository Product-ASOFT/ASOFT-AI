namespace ASOFT.CoreAI.Infrastructure
{
    public interface IPermissionHandler
    {
        Task<string> GetAccessType(List<string> permisions);

        List<string> GetPluginsUserHasAccess(string userId, IEnumerable<string> pluginCodesToCheck);
    }
}