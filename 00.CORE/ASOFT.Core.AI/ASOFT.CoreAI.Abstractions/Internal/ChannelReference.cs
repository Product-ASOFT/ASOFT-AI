// Copyright (c) Microsoft. All rights reserved.
using ASOFT.CoreAI.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace ASOFT.CoreAI.Agents.Abstractions.Internal;

/// <summary>
/// Tracks channel along with its hashed key.
/// </summary>
[Experimental("SKEXP0110")]
internal readonly struct ChannelReference(AgentChannel channel, string hash)
{
    /// <summary>
    /// The referenced channel.
    /// </summary>
    public AgentChannel Channel { get; } = channel;

    /// <summary>
    /// The channel hash.
    /// </summary>
    public string Hash { get; } = hash;
}