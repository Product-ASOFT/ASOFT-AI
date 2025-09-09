// Copyright (c) Microsoft. All rights reserved.
using ASOFT.CoreAI.Abstractions.PromptTemplate;
using ASOFT.CoreAI.Abstractions.Utilities;
using ASOFT.CoreAI.Common.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace ASOFT.CoreAI.Abstractions;

public abstract class Agent
{
    public string? Description { get; init; }
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string? Name { get; init; }
    public ILoggerFactory? LoggerFactory { get; init; }
    public KernelArguments? Arguments { get; init; }
    public string? Instructions { get; init; }
    public Kernel Kernel { get; init; } = new();
    public IPromptTemplate? Template { get; set; }

    public virtual IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> InvokeAsync(
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return this.InvokeAsync((ICollection<ChatMessageContent>)[], thread, options, cancellationToken);
    }

    public virtual IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> InvokeAsync(
        string message,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(message);
        return this.InvokeAsync(new ChatMessageContent(AuthorRole.User, message), thread, options, cancellationToken);
    }

    public virtual IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> InvokeAsync(
        ChatMessageContent message,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(message);
        return this.InvokeAsync([message], thread, options, cancellationToken);
    }

    public abstract IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> InvokeAsync(
        ICollection<ChatMessageContent> messages,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default);

    public virtual IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamingAsync(
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return this.InvokeStreamingAsync([], thread, options, cancellationToken);
    }

    public virtual IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamingAsync(
        string message,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(message);
        return this.InvokeStreamingAsync(new ChatMessageContent(AuthorRole.User, message), thread, options, cancellationToken);
    }

    public virtual IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamingAsync(
        ChatMessageContent message,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(message);
        return this.InvokeStreamingAsync([message], thread, options, cancellationToken);
    }

    public abstract IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamingAsync(
        ICollection<ChatMessageContent> messages,
        AgentThread? thread = null,
        AgentInvokeOptions? options = null,
        CancellationToken cancellationToken = default);

    protected virtual ILoggerFactory ActiveLoggerFactory => this.LoggerFactory ?? NullLoggerFactory.Instance;

    protected async Task<string?> RenderInstructionsAsync(Kernel kernel, KernelArguments? arguments, CancellationToken cancellationToken)
    {
        if (this.Template is null)
        {
            return this.Instructions;
        }
        var mergedArguments = this.Arguments.Merge(arguments);
        return await this.Template.RenderAsync(kernel, mergedArguments, cancellationToken).ConfigureAwait(false);
    }

    [Experimental("SKEXP0110")]
    protected internal abstract IEnumerable<string> GetChannelKeys();

#pragma warning restore CA1024

    [Experimental("SKEXP0110")]
    protected internal abstract Task<AgentChannel> CreateChannelAsync(CancellationToken cancellationToken);

    [Experimental("SKEXP0110")]
    protected internal abstract Task<AgentChannel> RestoreChannelAsync(string channelState, CancellationToken cancellationToken);

    public virtual async Task<TThreadType> EnsureThreadExistsWithMessagesAsync<TThreadType>(
        ICollection<ChatMessageContent> messages,
        AgentThread? thread,
        Func<TThreadType> constructThread,
        CancellationToken cancellationToken)
        where TThreadType : AgentThread
    {
        if (thread is null)
        {
            thread = constructThread();
        }
        if (thread is not TThreadType concreteThreadType)
        {
            throw new KernelException($"{this.GetType().Name} currently only supports agent threads of type {nameof(TThreadType)}.");
        }
        await thread.CreateAsync(cancellationToken).ConfigureAwait(false);
        foreach (var message in messages)
        {
            await this.NotifyThreadOfNewMessage(thread, message, cancellationToken).ConfigureAwait(false);
        }
        return concreteThreadType;
    }

    protected Task NotifyThreadOfNewMessage(AgentThread thread, ChatMessageContent message, CancellationToken cancellationToken)
    {
        return thread.OnNewMessageAsync(message, cancellationToken);
    }
}