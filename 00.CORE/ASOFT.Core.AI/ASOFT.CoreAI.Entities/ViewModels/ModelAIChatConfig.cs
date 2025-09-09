// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace ASOFT.CoreAI.Entities;

/// <summary>
/// OpenAI chat configuration.
/// </summary>
public sealed class ModelAIChatConfig
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string ConfigSectionName = "OpenAIChat";

    /// <summary>
    /// The name of the chat model.
    /// </summary>
    [Required]
    public string ModelName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
    public string ModelEmbedding { get; set; } = string.Empty;
}