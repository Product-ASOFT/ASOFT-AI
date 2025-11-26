namespace ASOFT.CoreAI.Infrastructure
{
    public interface IPermissionHandler
    {
        Task<string> GetAccessType(List<string> permisions);

    }
}