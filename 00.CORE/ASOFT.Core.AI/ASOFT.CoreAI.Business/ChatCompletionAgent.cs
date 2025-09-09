// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Abstractions.PromptTemplate;
using ASOFT.CoreAI.Business.Extensions;
using ASOFT.CoreAI.Business.Functions;
using ASOFT.CoreAI.Common.Diagnostics;
using ASOFT.CoreAI.Infrastructure;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace ASOFT.CoreAI.Business;

public sealed class ChatCompletionAgent : ChatHistoryAgent
{
    public ChatCompletionAgent()
    { }

    public ChatCompletionAgent(
        PromptTemplateConfig templateConfig,
        IPromptTemplateFactory templateFactory)
    {
        this.Name = templateConfig.Name;
        this.Description = templateConfig.Description;
        this.Instructions = templateConfig.Template;
        this.Arguments = new(templateConfig.ExecutionSettings.Values);
        this.Template = templateFactory.Create(templateConfig);
    }

    public AuthorRole InstructionsRole { get; init; } = AuthorRole.System;

    // hàm gọi xử lý khi người dùng đưa thông tin message vào.
    public override async IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> InvokeAsync(
        ICollection<ChatMessageContent> messages,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Verify.NotNull(messages);

        var chatHistoryAgentThread = await this.EnsureThreadExistsWithMessagesAsync(
            messages,
            thread,
            () => new ChatHistoryAgentThread(),
            cancellationToken).ConfigureAwait(false);

        var kernel = this.Kernel.Clone();

#pragma warning disable SKEXP0110
        var providersContext = await chatHistoryAgentThread.AIContextProviders.ModelInvokingAsync(messages, cancellationToken).ConfigureAwait(false);
        kernel.Plugins.AddFromAIContext(providersContext, "Tools");
#pragma warning restore SKEXP0110

        var chatHistory = new ChatHistory();
        await foreach (var existingMessage in chatHistoryAgentThread.GetMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            chatHistory.Add(existingMessage);
        }
        // xử lý kết quả gọi đến ModelAI
        var invokeResults = this.InternalInvokeAsync(
            this.GetDisplayName(),
            chatHistory,
            async (m) =>
            {
                await this.NotifyThreadOfNewMessage(chatHistoryAgentThread, m, cancellationToken).ConfigureAwait(false);
                if (options?.OnIntermediateMessage is not null)
                {
                    await options.OnIntermediateMessage(m).ConfigureAwait(false);
                }
            },
            options?.KernelArguments,
            kernel,
            options?.AdditionalInstructions == null ?
                providersContext.Instructions :
                string.Concat(options.AdditionalInstructions, Environment.NewLine, Environment.NewLine, providersContext.Instructions),
            cancellationToken);

        await foreach (var result in invokeResults.ConfigureAwait(false))
        {
            if (!result.Items.Any(i => i is FunctionCallContent || i is FunctionResultContent))
            {
                await this.NotifyThreadOfNewMessage(chatHistoryAgentThread, result, cancellationToken).ConfigureAwait(false);

                if (options?.OnIntermediateMessage is not null)
                {
                    await options.OnIntermediateMessage(result).ConfigureAwait(false);
                }
            }

            yield return new(result, chatHistoryAgentThread);
        }
    }

    [Obsolete("Use InvokeAsync with AgentThread instead. This method will be removed after May 1st 2025.")]
    public override IAsyncEnumerable<ChatMessageContent> InvokeAsync(
        ChatHistory history,
        KernelArguments? arguments = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        string agentName = this.GetDisplayName();

        return ActivityExtensions.RunWithActivityAsync(
            () => ModelDiagnostics.StartAgentInvocationActivity(this.Id, agentName, this.Description),
            () => this.InternalInvokeAsync(agentName, history, (m) => Task.CompletedTask, arguments, kernel, null, cancellationToken),
            cancellationToken);
    }

    public override async IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamingAsync(
        ICollection<ChatMessageContent> messages,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Verify.NotNull(messages);

        var chatHistoryAgentThread = await this.EnsureThreadExistsWithMessagesAsync(
            messages,
            thread,
            () => new ChatHistoryAgentThread(),
            cancellationToken).ConfigureAwait(false);

        var kernel = this.Kernel;

#pragma warning disable SKEXP0110
        var providersContext = await chatHistoryAgentThread.AIContextProviders.ModelInvokingAsync(messages, cancellationToken).ConfigureAwait(false);
        kernel.Plugins.AddFromAIContext(providersContext, "Tools");
#pragma warning restore SKEXP0110

        var chatHistory = new ChatHistory();
        await foreach (var existingMessage in chatHistoryAgentThread.GetMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            chatHistory.Add(existingMessage);
        }
        string agentName = this.GetDisplayName();
        var invokeResults = this.InternalInvokeStreamingAsync(
            agentName,
            chatHistory,
            async (m) =>
            {
                await this.NotifyThreadOfNewMessage(chatHistoryAgentThread, m, cancellationToken).ConfigureAwait(false);
                if (options?.OnIntermediateMessage is not null)
                {
                    await options.OnIntermediateMessage(m).ConfigureAwait(false);
                }
            },
            options?.KernelArguments,
            kernel,
            options?.AdditionalInstructions == null ?
                providersContext.Instructions :
                string.Concat(options.AdditionalInstructions, Environment.NewLine, Environment.NewLine, providersContext),
            cancellationToken);

        await foreach (var result in invokeResults.ConfigureAwait(false))
        {
            yield return new(result, chatHistoryAgentThread);
        }
    }

    [Obsolete("Use InvokeStreamingAsync with AgentThread instead. This method will be removed after May 1st 2025.")]
    public override IAsyncEnumerable<StreamingChatMessageContent> InvokeStreamingAsync(
        ChatHistory history,
        KernelArguments? arguments = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        string agentName = this.GetDisplayName();

        return ActivityExtensions.RunWithActivityAsync(
            () => ModelDiagnostics.StartAgentInvocationActivity(this.Id, agentName, this.Description),
            () => this.InternalInvokeStreamingAsync(
                agentName,
                history,
                (newMessage) => Task.CompletedTask,
                arguments,
                kernel,
                null,
                cancellationToken),
            cancellationToken);
    }

    protected override Task<AgentChannel> RestoreChannelAsync(string channelState, CancellationToken cancellationToken)
    {
        ChatHistory history =
            JsonSerializer.Deserialize<ChatHistory>(channelState) ??
            throw new KernelException("Unable to restore channel: invalid state.");
#pragma warning disable SKEXP0110
        return Task.FromResult<AgentChannel>(new ChatHistoryChannel(history));
#pragma warning restore SKEXP0110
    }

    internal static (IChatCompletionService service, PromptExecutionSettings? executionSettings) GetChatCompletionService(Kernel kernel, KernelArguments? arguments)
    {
        KernelFunction nullPrompt = KernelFunctionFactory.CreateFromPrompt("placeholder", arguments?.ExecutionSettings?.Values);

        kernel.ServiceSelector.TrySelectAIService<IChatCompletionService>(kernel, nullPrompt, arguments ?? [], out IChatCompletionService? chatCompletionService, out PromptExecutionSettings? executionSettings);

#pragma warning disable CA2000
        if (chatCompletionService is null
    && kernel.ServiceSelector is IChatClientSelector chatClientSelector
    && chatClientSelector.TrySelectChatClient<Microsoft.Extensions.AI.IChatClient>(kernel, nullPrompt, arguments ?? [], out var chatClient, out executionSettings)
    && chatClient is not null)
        {
        }
#pragma warning restore CA2000

        if (chatCompletionService is null)
        {
            var message = new StringBuilder().Append("No service was found for any of the supported types: ").Append(typeof(IChatCompletionService)).Append(", ").Append(typeof(Microsoft.Extensions.AI.IChatClient)).Append('.');
            if (nullPrompt.ExecutionSettings is not null)
            {
                string serviceIds = string.Join("|", nullPrompt.ExecutionSettings.Keys);
                if (!string.IsNullOrEmpty(serviceIds))
                {
                    message.Append(" Expected serviceIds: ").Append(serviceIds).Append('.');
                }

                string modelIds = string.Join("|", nullPrompt.ExecutionSettings.Values.Select(model => model.ModelId));
                if (!string.IsNullOrEmpty(modelIds))
                {
                    message.Append(" Expected modelIds: ").Append(modelIds).Append('.');
                }
            }

            throw new KernelException(message.ToString());
        }

        return (chatCompletionService!, executionSettings);
    }

    #region private

    private async Task<ChatHistory> SetupAgentChatHistoryAsync(
        IReadOnlyList<ChatMessageContent> history,
        KernelArguments? arguments,
        Kernel kernel,
        string? additionalInstructions,
        CancellationToken cancellationToken)
    {
        ChatHistory chat = [];

        string? instructions = await this.RenderInstructionsAsync(kernel, arguments, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(instructions))
        {
            chat.Add(new ChatMessageContent(this.InstructionsRole, instructions) { AuthorName = this.Name });
        }

        if (!string.IsNullOrWhiteSpace(additionalInstructions))
        {
            chat.Add(new ChatMessageContent(AuthorRole.System, additionalInstructions) { AuthorName = this.Name });
        }

        chat.AddRange(history);

        return chat;
    }

    private async IAsyncEnumerable<ChatMessageContent> InternalInvokeAsync(
        string agentName,
        ChatHistory history,
        Func<ChatMessageContent, Task> onNewToolMessage,
        KernelArguments? arguments = null,
        Kernel? kernel = null,
        string? additionalInstructions = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        kernel ??= this.Kernel;

        (IChatCompletionService chatCompletionService, PromptExecutionSettings? executionSettings) = GetChatCompletionService(kernel, this.Arguments.MergeArguments(arguments));

        ChatHistory chat = await this.SetupAgentChatHistoryAsync(history, arguments, kernel, additionalInstructions, cancellationToken).ConfigureAwait(false);

        int messageCount = chat.Count;

        Type serviceType = chatCompletionService.GetType();
        IReadOnlyList<ChatMessageContent> messages =
            await chatCompletionService.GetChatMessageContentsAsync(
                chat,
                executionSettings,
                kernel,
                cancellationToken).ConfigureAwait(false);

        for (int messageIndex = messageCount; messageIndex < chat.Count; messageIndex++)
        {
            ChatMessageContent message = chat[messageIndex];

            message.AuthorName = this.Name;

            history.Add(message);
            await onNewToolMessage(message).ConfigureAwait(false);
        }

        foreach (ChatMessageContent message in messages)
        {
            message.AuthorName = this.Name;

            yield return message;
        }
    }

    private async IAsyncEnumerable<StreamingChatMessageContent> InternalInvokeStreamingAsync(
        string agentName,
        ChatHistory history,
        Func<ChatMessageContent, Task> onNewMessage,
        KernelArguments? arguments = null,
        Kernel? kernel = null,
        string? additionalInstructions = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        kernel ??= this.Kernel;

        (IChatCompletionService chatCompletionService, PromptExecutionSettings? executionSettings) = GetChatCompletionService(kernel, this.Arguments.MergeArguments(arguments));

        ChatHistory chat = await this.SetupAgentChatHistoryAsync(history, arguments, kernel, additionalInstructions, cancellationToken).ConfigureAwait(false);

        int messageCount = chat.Count;

        Type serviceType = chatCompletionService.GetType();

        IAsyncEnumerable<StreamingChatMessageContent> messages =
            chatCompletionService.GetStreamingChatMessageContentsAsync(
                chat,
                executionSettings,
                kernel,
                cancellationToken);

        AuthorRole? role = null;
        StringBuilder builder = new();
        await foreach (StreamingChatMessageContent message in messages.ConfigureAwait(false))
        {
            role = message.Role;
            message.Role ??= AuthorRole.Assistant;
            message.AuthorName = this.Name;

            builder.Append(message.ToString());

            yield return message;
        }

        for (int messageIndex = messageCount; messageIndex < chat.Count; messageIndex++)
        {
            ChatMessageContent message = chat[messageIndex];

            message.AuthorName = this.Name;

            await onNewMessage(message).ConfigureAwait(false);
            history.Add(message);
        }

        if (role != AuthorRole.Tool)
        {
            await onNewMessage(new(role ?? AuthorRole.Assistant, builder.ToString()) { AuthorName = this.Name }).ConfigureAwait(false);
            history.Add(new(role ?? AuthorRole.Assistant, builder.ToString()) { AuthorName = this.Name });
        }
    }

    #endregion private
}