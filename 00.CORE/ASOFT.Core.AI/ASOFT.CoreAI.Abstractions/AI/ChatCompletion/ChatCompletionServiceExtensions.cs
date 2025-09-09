// Copyright (c) Microsoft. All rights reserved.

namespace ASOFT.CoreAI.Abstractions;

/// <summary>
/// Class sponsor that holds extension methods for <see cref="IChatCompletionService"/> interface.
/// </summary>
public static class ChatCompletionServiceExtensions
{
    public static Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        this IChatCompletionService chatCompletionService,
        string prompt,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        // Try to parse the text as a chat history
        if (ChatPromptParser.TryParse(prompt, out var chatHistoryFromPrompt))
        {
            return chatCompletionService.GetChatMessageContentsAsync(chatHistoryFromPrompt, executionSettings, kernel, cancellationToken);
        }

        // Otherwise, use the prompt as the chat user message
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);

        return chatCompletionService.GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
    }

    public static IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        this IChatCompletionService chatCompletionService,
        string prompt,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        // Try to parse the text as a chat history
        if (ChatPromptParser.TryParse(prompt, out var chatHistoryFromPrompt))
        {
            return chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistoryFromPrompt, executionSettings, kernel, cancellationToken);
        }

        // Otherwise, use the prompt as the chat user message
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);

        return chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
    }
}