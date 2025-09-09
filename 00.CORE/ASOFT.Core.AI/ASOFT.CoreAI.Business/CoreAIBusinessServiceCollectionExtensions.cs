using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Loader;

namespace ASOFT.CoreAI.Business
{
    public static class CoreAIBusinessServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            //services.AddMediatR(typeof(TaskAgentHandler).Assembly);
            services.AddScoped<AgentManager, AgentManager>();
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //services.AddMediatR(assemblies);

            //#region Tìm thư mục chứa plugin

            //string baseDir = AppContext.BaseDirectory;
            //var dir = new DirectoryInfo(baseDir);
            //while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "00.CORE")))
            //{
            //    dir = dir.Parent;
            //}
            //string rootPath = dir?.FullName ?? string.Empty;
            //string pluginsPath = Path.Combine(rootPath, "00.CORE", "ASOFT.Core.AI", "ASOFT.CoreAI.Business", "PluginAgent");

            //if (!Directory.Exists(pluginsPath))
            //{
            //    return services;
            //}

            //#endregion Tìm thư mục chứa plugin

            //var dllFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly);
            //foreach (var dllPath in dllFiles)
            //{
            //    // Load assembly động
            //    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
            //    // Đăng ký MediatR handler trong assembly
            //    services.AddMediatR(assembly);
            //}
            return services;
        }
    }
}