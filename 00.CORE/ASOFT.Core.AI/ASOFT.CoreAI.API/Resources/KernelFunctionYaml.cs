// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Abstractions.PromptTemplate;
using ASOFT.CoreAI.Business.Functions;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ASOFT.CoreAI.API.Resources;

/// <summary>
/// Factory methods for creating <seealso cref="KernelFunction"/> instances.
/// </summary>
public static class KernelFunctionYaml
{
    /// <summary>
    /// Creates a <see cref="KernelFunction"/> instance for a prompt function using the specified markdown text.
    /// </summary>
    /// <param name="text">YAML representation of the <see cref="PromptTemplateConfig"/> to use to create the prompt function.</param>
    /// <param name="promptTemplateFactory">
    /// The <see cref="IPromptTemplateFactory"/> to use when interpreting the prompt template configuration into a <see cref="IPromptTemplate"/>.
    /// If null, a default factory will be used.
    /// </param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for logging. If null, no logging will be performed.</param>
    /// <returns>The created <see cref="KernelFunction"/>.</returns>
    public static KernelFunction FromPromptYaml(
        string text,
        IPromptTemplateFactory? promptTemplateFactory = null,
        ILoggerFactory? loggerFactory = null)
    {
        PromptTemplateConfig promptTemplateConfig = ToPromptTemplateConfig(text);

        foreach (var inputVariable in promptTemplateConfig.InputVariables)
        {
            if (inputVariable.Default is not null and not string)
            {
                throw new NotSupportedException($"Default value for input variable '{inputVariable.Name}' must be a string. " +
                        $"This is a temporary limitation; future updates are expected to remove this constraint. Prompt function - '{promptTemplateConfig.Name ?? promptTemplateConfig.Description}'.");
            }
        }

        return KernelFunctionFactory.CreateFromPrompt(promptTemplateConfig, promptTemplateFactory, loggerFactory);
    }

    /// <summary>
    /// Convert the given YAML text to a <see cref="PromptTemplateConfig"/> model.
    /// </summary>
    /// <param name="text">YAML representation of the <see cref="PromptTemplateConfig"/> to use to create the prompt function.</param>
    public static PromptTemplateConfig ToPromptTemplateConfig(string text)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTypeConverter(new PromptExecutionSettingsTypeConverter())
            .IgnoreUnmatchedProperties()
            .Build();
        return deserializer.Deserialize<PromptTemplateConfig>(text);
    }
}