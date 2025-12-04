using ASOFT.Core.API.Extensions;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using ASOFT.CoreAI.API.Resources;
using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Business.LibraryKernel;
using ASOFT.CoreAI.Business.Services.BackgroudJobHandler;
using ASOFT.CoreAI.Business.Services.ChatHandler;
using ASOFT.CoreAI.Business.Services.ChatHandler.ChatStorage;
using ASOFT.CoreAI.Business.Services.RedisHandler;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Kernel = ASOFT.CoreAI.Abstractions.Kernel;

[assembly: HostingStartup(typeof(AIHostingStartup))]

public class AIHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((ctx, services) =>
        {
            ConfigureCoreServices(ctx, services);
            ConfigureRedisServices(services);
            AddAIServices(services);
            AddAgent(services);
            AddServiceChatHistory(services);
        });
    }

    #region Core services

    private static void ConfigureCoreServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        var configuration = ctx.Configuration;

        // ASP.NET Core MVC + load assembly hiện tại
        services.AddApiMvc(ctx.HostingEnvironment)
                .AddApplicationPart(typeof(AIHostingStartup).Assembly);

        services.AddControllers();

        // Kernel + OpenAI pipeline chung
        services.AddAsoftKernel();

        // Business / Query services
        services.AddScoped<IPermissionHandler, PermissionService>();
        services.AddScoped<IST2130Queries, ST2130Queries>();
        services.AddScoped<IST2131Queries, ST2131Queries>();
        services.AddScoped<IST2136Queries, ST2136Queries>();
        services.AddScoped<IONT1021Service, ONT1021Service>();
        services.AddScoped<IONT1030Service, ONT1030Service>();
        services.AddScoped<IDataLoader, DataLoaderService>();

        // Embedding + Redis
        services.AddScoped<IOpenAIEmbeddingService, OpenAIEmbeddingService>();
        services.AddScoped<IRedisService, RedisService>();

        // Các service nghiệp vụ khác
        services.AddScoped<SettingsManagerService>();
        services.AddScoped<ICIF1640DAL, CIF1640DAL>();
        services.AddScoped<AgentManagerService>();
        services.AddScoped<IOCRService, OcrService>();
        services.AddScoped<ITrainingDataService, TrainingDataService>();
        services.AddScoped<FilePathService>();
        services.AddScoped<AgentCompareService>();
        services.AddScoped<ReadFileOrchestratorService>();
        services.AddScoped<AgentPromptService>();
        services.AddScoped<IReadFileBackgroundWorkflow, ReadFileBackgroundWorkflow>();

        // Job queue + worker
        services.AddSingleton<IJobQueue>(_ => new ChannelJobQueue(capacity: 200));
        services.AddHostedService<ReadFileWorker>();

        // Redis IDatabase (lấy từ IConnectionMultiplexer)
        services.AddScoped<IDatabase>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        // Đăng ký dbset cho entities
        services.AddTransient<IModelBuilderConfiguration<BusinessDbContext>, ModuleCoreAIModelBuilderConfiguration>();

        // HttpClient cho OCR
        services.AddHttpClient("OCR", client =>
        {
            client.Timeout = TimeSpan.FromMinutes(15);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            ResponseDrainTimeout = TimeSpan.FromSeconds(30)
        });
    }

    #endregion

    #region AI services (Chat + Embedding)

    private static void AddAIServices(IServiceCollection services)
    {
        // 1. Cấu hình lưu trữ config model
        services.AddScoped<IAIConfigStore, AIConfigStore>();
        services.AddScoped<IRedisMemoryProvider, RedisMemoryProvider>();
        services.AddScoped<IOpenAIClientProvider, OpenAIClientProvider>();

        // 2. ChatCompletion (model lấy từ IAIConfigStore)
        services.AddOpenAIChatCompletion();

        // 3. OpenAIEmbeddingService đã đăng ký qua IOpenAIEmbeddingService ở trên
        // Nếu cần resolve trực tiếp concrete:
        services.AddScoped<OpenAIEmbeddingService>();
    }

    #endregion

    #region Chat history

    private static void AddServiceChatHistory(IServiceCollection services)
    {
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IChatResponseRepository, ChatResponseRepository>();
        services.AddScoped<IChatFileRepository, ChatFileRepository>();
        services.AddScoped<IChatHistoryHandler, ChatHistoryHandler>();
    }

    #endregion

    #region Agent

    private static void AddAgent(IServiceCollection services)
    {
        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(
            EmbeddedResource.Read("AgentDefinition.yaml"));

        // Chỉ đăng ký 1 lần ChatCompletionAgent bằng factory
        services.AddTransient<ChatCompletionAgent>(sp =>
        {
            return new ChatCompletionAgent(templateConfig, new HandlebarsPromptTemplateFactory())
            {
                Kernel = sp.GetRequiredService<Kernel>()
            };
        });
    }

    #endregion

    #region Redis

    private static void ConfigureRedisServices(IServiceCollection services)
    {
        // Các service đọc cấu hình Redis
        services.AddScoped<ConfigManagerService>();
        services.AddScoped<IRedisConfigProvider, RedisConfigProvider>();

        // IConnectionMultiplexer dùng singleton – logic tạo gom vào 1 hàm cho gọn
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            // Cần scope tạm vì RedisConfigProvider là scoped
            using var scope = sp.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            var redisConfigProvider = scopedProvider.GetRequiredService<IRedisConfigProvider>();
            var redisConfig = redisConfigProvider.GetRedisConfigAsync().GetAwaiter().GetResult();

            if (redisConfig == null || string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
                throw new InvalidOperationException("Redis connection string is missing.");

            var options = ConfigurationOptions.Parse(redisConfig.ConnectionString);

            if (!string.IsNullOrEmpty(redisConfig.UserName))
                options.User = redisConfig.UserName;

            if (!string.IsNullOrEmpty(redisConfig.Password))
                options.Password = redisConfig.Password;

            if (!string.IsNullOrEmpty(redisConfig.DatabaseName) &&
                int.TryParse(redisConfig.DatabaseName, out var db))
            {
                options.DefaultDatabase = db;
            }

            options.SyncTimeout = 30000;
            options.AsyncTimeout = 30000;
            options.ConnectTimeout = 30000;
            options.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(options);
        });
    }

    #endregion
}
