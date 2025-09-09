// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Common.Diagnostics;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.AI;

namespace ASOFT.CoreAI.Abstractions.ChatClient;

public sealed class ChatClientAIService : IAIService
{
    private readonly IChatClient _chatClient;

    internal Dictionary<string, object?> _internalAttributes { get; } = [];

    public ChatClientAIService(IChatClient chatClient)
    {
        Verify.NotNull(chatClient);
        this._chatClient = chatClient;

        var metadata = this._chatClient.GetService<ChatClientMetadata>();
        Verify.NotNull(metadata);

        this._internalAttributes[AIServiceExtensions.ModelIdKey] = metadata.DefaultModelId;
        this._internalAttributes[nameof(metadata.ProviderName)] = metadata.ProviderName;
        this._internalAttributes[nameof(metadata.ProviderUri)] = metadata.ProviderUri;
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => this._internalAttributes;

    /// <inheritdoc />
}