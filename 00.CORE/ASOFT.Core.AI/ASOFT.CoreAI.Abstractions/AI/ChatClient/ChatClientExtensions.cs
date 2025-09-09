// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Common.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.CoreAI.Abstractions.ChatClient;

/// <summary>Provides extension methods for <see cref="IChatClient"/>.</summary>
public static class ChatClientExtensions
{
    public static Task<ChatResponse> GetResponseAsync(
        this IChatClient chatClient,
        string prompt,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        var chatOptions = executionSettings.ToChatOptions(kernel);

        // Try to parse the text as a chat history
        if (ChatPromptParser.TryParse(prompt, out var chatHistoryFromPrompt))
        {
            var messageList = chatHistoryFromPrompt.ToChatMessageList();
            return chatClient.GetResponseAsync(messageList, chatOptions, cancellationToken);
        }

        return chatClient.GetResponseAsync(prompt, chatOptions, cancellationToken);
    }

    public static string? GetModelId(this IChatClient client)
    {
        Verify.NotNull(client);
        return client.GetService<ChatClientMetadata>()?.DefaultModelId;
    }
}