// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Abstractions;

/// <summary>
/// Interface for chat completion services.
/// </summary>
public interface IChatCompletionService : IAIService
{
    Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default);
}