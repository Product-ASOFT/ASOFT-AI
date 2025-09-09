// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Common.Diagnostics;
using System.Text.Json.Serialization;

namespace ASOFT.CoreAI.Abstractions;

/// <summary>
/// A description of the intended purpose of a message within a chat completions interaction.
/// </summary>
public readonly struct AuthorRole : IEquatable<AuthorRole>
{
    /// <summary>
    /// The role that instructs or sets the behavior of the assistant.
    /// </summary>
    public static AuthorRole Developer { get; } = new("developer");

    /// <summary>
    /// The role that instructs or sets the behavior of the assistant.
    /// </summary>
    public static AuthorRole System { get; } = new("system");

    /// <summary>
    /// The role that provides responses to system-instructed, user-prompted input.
    /// </summary>
    public static AuthorRole Assistant { get; } = new("assistant");

    /// <summary>
    /// The role that provides input for chat completions.
    /// </summary>
    public static AuthorRole User { get; } = new("user");

    /// <summary>
    /// The role that provides additional information and references for chat completions.
    /// </summary>
    public static AuthorRole Tool { get; } = new("tool");

    public string Label { get; }

    [JsonConstructor]
    public AuthorRole(string label)
    {
        Verify.NotNullOrWhiteSpace(label, nameof(label));
        this.Label = label!;
    }

    public static bool operator ==(AuthorRole left, AuthorRole right)
        => left.Equals(right);

    public static bool operator !=(AuthorRole left, AuthorRole right)
        => !(left == right);

    public bool Equals(AuthorRole other)
        => string.Equals(this.Label, other.Label, StringComparison.OrdinalIgnoreCase);
}