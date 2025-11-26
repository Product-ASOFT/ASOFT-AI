// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.
// #
// # History：
// #	Date Time	    Updated		    Content
// #    10/07/2025      Đức Mạnh        Tạo mới
// ##################################################################

using ASOFT.Core.API.Extensions;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using ASOFT.CoreAI.API.Resources;
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
    // Constructor public nhận IConnectionMultiplexer từ DI

    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((ctx, services) =>
        {
            var configuration = ctx.Configuration;

            ConfigureCoreServices(ctx, services);

            ConfigureRedisServices(services);

            AddAIServices(services);

            AddAgent(services);

            AddServiceChatHistory(services);
        });
    }

    private static void ConfigureCoreServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        var configuration = ctx.Configuration;

        services.AddApiMvc(ctx.HostingEnvironment)
                .AddApplicationPart(typeof(AIHostingStartup).Assembly);

        services.AddControllers();

        // Thêm các dịch cho OpenAI
        services.AddAsoftKernel();

        services.AddTransient<ChatCompletionAgent>();
        services.AddScoped<IPermissionHandler, PermissionService>();
        services.AddScoped<IST2130Queries, ST2130Queries>();
        services.AddScoped<IST2131Queries, ST2131Queries>();
        services.AddScoped<IST2136Queries, ST2136Queries>();
        services.AddScoped<IONT1021Service, ONT1021Service>();
        services.AddScoped<IONT1030Service, ONT1030Service>();
        services.AddScoped<IDataLoader, DataLoaderService>();
        services.AddScoped<IOpenAIEmbeddingService, OpenAIEmbeddingService>();
        services.AddScoped<IRedisService, RedisService>();
        services.AddScoped<SettingsManagerService>();
        services.AddScoped<ICIF1640DAL, CIF1640DAL>();
        services.AddScoped<AgentManagerService>();
        services.AddScoped<IOCRService, OcrService>();
        services.AddScoped<ITrainingDataService, TrainingDataService>();
        services.AddScoped<FilePathService>();
        services.AddScoped<AgentCompareService>();
        services.AddScoped<ReadFileOrchestratorService>();
        services.AddScoped<AgentPromptService>();
        services.AddScoped<ReadFileOrchestratorService>();
        services.AddSingleton<IJobQueue>(sp => new ChannelJobQueue(capacity: 200));
        services.AddHostedService<ReadFileWorker>();
        services.AddScoped<IReadFileBackgroundWorkflow, ReadFileBackgroundWorkflow>();

        services.AddScoped<IDatabase>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        // Đăng ký dbset cho entities
        services.AddTransient<IModelBuilderConfiguration<BusinessDbContext>, ModuleCoreAIModelBuilderConfiguration>();

        // Đăng ký các dịch vụ cho MediatR
        //services.AddCoreApplicationServices();

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

    private static void AddAIServices(IServiceCollection services)
    {
        // 1. Đăng ký các dịch vụ cấu hình cho ModelAI
        services.AddScoped<IAIConfigStore, AIConfigStore>();
        services.AddScoped<IRedisMemoryProvider, RedisMemoryProvider>();
        services.AddScoped<IOpenAIClientProvider, OpenAIClientProvider>();

        // 2. Đăng ký dịch vụ chat completion, không truyền model mặc định vì lấy từ IAIConfigStore
        services.AddOpenAIChatCompletion();

        // 3. Đăng ký OpenAIEmbeddingService scoped (như hiện tại)
        services.AddScoped<OpenAIEmbeddingService>();
    }

    private static void AddServiceChatHistory(IServiceCollection services)
    {
        // Đăng ký dịch vụ lịch sử trò chuyện
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IChatResponseRepository, ChatResponseRepository>();
        services.AddScoped<IChatFileRepository, ChatFileRepository>();
        services.AddScoped<IChatHistoryHandler, ChatHistoryHandler>();
    }

    private static void AddAgent(IServiceCollection services)
    {
        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(EmbeddedResource.Read("AgentDefinition.yaml"));

        services.AddTransient<ChatCompletionAgent>(sp =>
        {
            return new ChatCompletionAgent(templateConfig, new HandlebarsPromptTemplateFactory())
            {
                Kernel = sp.GetRequiredService<Kernel>()
            };
        });
    }
    public void ConfigureRedisServices(IServiceCollection services)
    {
        services.AddScoped<ConfigManagerService>();
        services.AddScoped<IRedisConfigProvider, RedisConfigProvider>();
        // Đăng ký IConnectionMultiplexer trong DI container
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            using var scope = sp.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            var redisConfigProvider = scopedProvider.GetRequiredService<IRedisConfigProvider>();

            var redisConfig = redisConfigProvider.GetRedisConfigAsync().GetAwaiter().GetResult();

            if (redisConfig == null || string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
                throw new InvalidOperationException("Redis connection string is missing.");

            var redisOptions = ConfigurationOptions.Parse(redisConfig.ConnectionString);

            if (!string.IsNullOrEmpty(redisConfig.UserName))
                redisOptions.User = redisConfig.UserName;

            if (!string.IsNullOrEmpty(redisConfig.Password))
                redisOptions.Password = redisConfig.Password;

            if (!string.IsNullOrEmpty(redisConfig.DatabaseName)
                && int.TryParse(redisConfig.DatabaseName, out var db))
            {
                redisOptions.DefaultDatabase = db;
            }

            redisOptions.SyncTimeout = 30000;
            redisOptions.AsyncTimeout = 30000;
            redisOptions.ConnectTimeout = 30000;
            redisOptions.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(redisOptions);
        });
    }
}