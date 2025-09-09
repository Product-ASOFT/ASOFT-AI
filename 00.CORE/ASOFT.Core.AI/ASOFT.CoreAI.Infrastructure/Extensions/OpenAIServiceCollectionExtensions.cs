// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Common.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ASOFT.CoreAI.Infrastructure;

#pragma warning disable IDE0039 // Use local function

/// <summary>
/// Sponsor extensions clas#pragma warning disable IDE0039

public static partial class OpenAIServiceCollectionExtensions
{
    public static OpenAIFunction ToOpenAIFunction(this KernelFunctionMetadata metadata)
    {
        IReadOnlyList<KernelParameterMetadata> metadataParams = metadata.Parameters;

        var openAIParams = new OpenAIFunctionParameter[metadataParams.Count];
        for (int i = 0; i < openAIParams.Length; i++)
        {
            var param = metadataParams[i];

            openAIParams[i] = new OpenAIFunctionParameter(
                param.Name,
                GetDescription(param),
                param.IsRequired,
                param.ParameterType,
                param.Schema);
        }

        return new OpenAIFunction(
            metadata.PluginName,
            metadata.Name,
            metadata.Description,
            openAIParams,
            new OpenAIFunctionReturnParameter(
                metadata.ReturnParameter.Description,
                metadata.ReturnParameter.ParameterType,
                null));

        static string GetDescription(KernelParameterMetadata param)
        {
            if (InternalTypeConverter.ConvertToString(param.DefaultValue) is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                return $"{param.Description} (default value: {stringValue})";
            }

            return param.Description;
        }
    }

    public static IServiceCollection AddOpenAIChatCompletion(this IServiceCollection services, string? serviceId = null)
    {
        Verify.NotNull(services);
        OpenAIChatCompletionService Factory(IServiceProvider sp, object? _)
        {
            var configStore = sp.GetRequiredService<IAIConfigStore>();
            var clientProvider = sp.GetRequiredService<IOpenAIClientProvider>();

            if (string.IsNullOrWhiteSpace(configStore.GetApiKey()) ||
                string.IsNullOrWhiteSpace(configStore.GetModelName()))
            {
                if (configStore is AIConfigStore redisStore)
                {
                    redisStore.LoadConfigFromRedisAsync().GetAwaiter().GetResult();
                }
                else
                {
                    throw new InvalidOperationException("ConfigStore chưa load được config.");
                }
            }

            var client = clientProvider.GetClient();
            if (client == null)
                throw new InvalidOperationException("OpenAIClient chưa được cấu hình.");

            var modelId = configStore.GetModelName();
            if (string.IsNullOrWhiteSpace(modelId))
                throw new InvalidOperationException("ModelName chưa được cấu hình.");

            var loggerFactory = sp.GetService<ILoggerFactory>();

            return new OpenAIChatCompletionService(modelId, client, loggerFactory);
        }

        services.AddKeyedScoped<IChatCompletionService>(serviceId, Factory); // Đổi thành scoped
        return services;
    }
}