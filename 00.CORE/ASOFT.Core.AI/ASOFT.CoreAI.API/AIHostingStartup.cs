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
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        services.AddScoped<IPermissionHandler, PermissionHandler>();
        services.AddScoped<IST2111Queries, ST2111Queries>();
        services.AddScoped<IST2121Queries, ST2121Queries>();
        services.AddScoped<IDataLoader, DataLoader>();
        services.AddScoped<IOpenAIEmbeddingService, OpenAIEmbeddingService>();
        services.AddScoped<IRedisHandler, RedisHandler>();
        services.AddScoped<SettingsManager>();
        services.AddScoped<OcrService>();
        services.AddScoped<ICIF1640DAL, CIF1640DAL>();
        services.AddScoped<AgentManager>();

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

    // Dùng khi có tích hợp RAG
    //    public static void AddAgentWithRag<TKey>(WebApplicationBuilder builder, PromptTemplateConfig templateConfig)
    //    {
    //        builder.Services.AddTransient<ChatCompletionAgent>(sp =>
    //        {
    //            // Lấy các service cần thiết từ DI container
    //            Kernel kernel = sp.GetRequiredService<Kernel>();
    //#pragma warning disable SKEXP0001
    //            // code sử dụng VectorStoreTextSearch
    //            ASOFT.CoreAI.Infrastructure.VectorStoreTextSearch<TextSnippet<TKey>> vectorStoreTextSearch = sp.GetRequiredService<ASOFT.CoreAI.Infrastructure.VectorStoreTextSearch<TextSnippet<TKey>>>();

    //            // Tạo plugin vector search và thêm vào kernel với tên "SearchPlugin"
    //            kernel.Plugins.Add(vectorStoreTextSearch.CreateWithGetTextSearchResults("SearchPlugin"));
    //#pragma warning restore SKEXP0001

    //            // Tạo instance agent, gán Kernel và template prompt factory
    //            return new ChatCompletionAgent(templateConfig, new HandlebarsPromptTemplateFactory())
    //            {
    //                Kernel = kernel,
    //            };
    //        });
    //    }
}

//public static class KernelFunctionYaml
//{
//    public static KernelFunction FromPromptYaml(
//        string text,
//        IPromptTemplateFactory? promptTemplateFactory = null,
//        ILoggerFactory? loggerFactory = null)
//    {
//        PromptTemplateConfig promptTemplateConfig = ToPromptTemplateConfig(text);

//        foreach (var inputVariable in promptTemplateConfig.InputVariables)
//        {
//            if (inputVariable.Default is not null and not string)
//            {
//                throw new NotSupportedException($"Default value for input variable '{inputVariable.Name}' must be a string. " +
//                        $"This is a temporary limitation; future updates are expected to remove this constraint. Prompt function - '{promptTemplateConfig.Name ?? promptTemplateConfig.Description}'.");
//            }
//        }

//        return KernelFunctionFactory.CreateFromPrompt(promptTemplateConfig, promptTemplateFactory, loggerFactory);
//    }

//    /// <summary>
//    /// Convert the given YAML text to a <see cref="PromptTemplateConfig"/> model.
//    /// </summary>
//    /// <param name="text">YAML representation of the <see cref="PromptTemplateConfig"/> to use to create the prompt function.</param>
//    public static PromptTemplateConfig ToPromptTemplateConfig(string text)
//    {
//        var deserializer = new DeserializerBuilder()
//            .WithNamingConvention(UnderscoredNamingConvention.Instance)
//            .WithTypeConverter(new PromptExecutionSettingsTypeConverter())
//            .Build();

//        return deserializer.Deserialize<PromptTemplateConfig>(text);
//    }
//}