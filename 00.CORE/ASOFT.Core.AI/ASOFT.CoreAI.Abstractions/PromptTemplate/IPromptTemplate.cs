// Copyright (c) Microsoft. All rights reserved.

namespace ASOFT.CoreAI.Abstractions.PromptTemplate;

/// <summary>
/// Represents a prompt template that can be rendered to a string.
/// </summary>
public interface IPromptTemplate
{
    Task<string> RenderAsync(Kernel kernel, KernelArguments? arguments = null, CancellationToken cancellationToken = default);
}