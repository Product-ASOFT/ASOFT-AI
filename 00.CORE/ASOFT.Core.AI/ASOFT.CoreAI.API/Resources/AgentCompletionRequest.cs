// Copyright (c) Microsoft. All rights reserved.

// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Abstractions;

namespace ASOFT.CoreAI.API.Resources;

/// <summary>
/// The agent completion request model.
/// </summary>
public sealed class AgentCompletionRequest
{
    /// <summary>
    /// Gets or sets the prompt.
    /// </summary>
    public string Prompt { get; set; }

    /// <summary>
    /// Gets or sets the chat history.
    /// </summary>
    public ChatHistory ChatHistory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether streaming is requested.
    /// </summary>
    public bool IsStreaming { get; set; }
}