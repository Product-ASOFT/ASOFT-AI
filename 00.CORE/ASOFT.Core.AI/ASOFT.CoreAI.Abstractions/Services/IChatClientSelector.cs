// Copyright (c) Microsoft. All rights reserved.
using ASOFT.CoreAI.Abstractions;
using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;

namespace ASOFT.CoreAI.Infrastructure;

#pragma warning disable CA1716 // Identifiers should not match keywords

/// <summary>
/// Represents a selector which will return a combination of the containing instances of T and it's pairing <see cref="PromptExecutionSettings"/>
/// from the specified provider based on the model settings.
/// </summary>
public interface IChatClientSelector
{
    bool TrySelectChatClient<T>(
        Kernel kernel,
        KernelFunction function,
        KernelArguments arguments,
        [NotNullWhen(true)] out T? service,
        out PromptExecutionSettings? serviceSettings) where T : class, IChatClient;
}