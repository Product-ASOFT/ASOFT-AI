// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.
// #
// # History：
// #	Date Time	    Updated		    Content
// ##################################################################

using ASOFT.Core.API.Extensions;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using ASOFT.CoreAI.API.Resources;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Business.LibraryKernel;
using ASOFT.CoreAI.Business.Services.ChatHandler;
using ASOFT.CoreAI.Business.Services.ChatHandler.ChatStorage;
using ASOFT.CoreAI.Business.Services.ChatHandler.FileStorage;
using ASOFT.CoreAI.Business.Services.RedisHandler;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
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

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var redisConfigString = configuration.GetValue<string>(AIConstants.RedisConfig);

            if (string.IsNullOrWhiteSpace(redisConfigString))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Redis config string is missing.");
                Console.ResetColor();
                throw new InvalidOperationException("Redis configuration is required.");
            }

            var redisConfig = ConfigurationOptions.Parse(redisConfigString);
            redisConfig.User = "default";
            redisConfig.Password = "asd@123";
            redisConfig.SyncTimeout = 30000;
            redisConfig.AsyncTimeout = 30000;
            redisConfig.ConnectTimeout = 30000;
            redisConfig.AbortOnConnectFail = false;
            redisConfig.DefaultDatabase = 0; // Chọn database 0 làm mặc định
            // hàm
            try
            {
                var connection = ConnectionMultiplexer.Connect(redisConfig);

                connection.ConnectionFailed += (s, e) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Redis Connection Failed: Endpoint={e.EndPoint}, Type={e.FailureType}, Message={e.Exception?.Message}");
                    Console.ResetColor();
                };

                connection.ConnectionRestored += (s, e) =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✅ Redis Connection Restored: Endpoint={e.EndPoint}");
                    Console.ResetColor();
                };

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Redis connected successfully.");
                Console.ResetColor();

                return connection;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"🚨 Failed to connect to Redis: {ex.Message}");
                Console.ResetColor();
                throw;
            }
        });

        services.AddScoped<IDatabase>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        // Đăng ký dbset cho entities
        services.AddTransient<IModelBuilderConfiguration<BusinessDbContext>, ModuleCoreAIModelBuilderConfiguration>();

        // Đăng ký các dịch vụ cho MediatR
        //services.AddCoreApplicationServices();

        services.AddHttpClient();
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
}