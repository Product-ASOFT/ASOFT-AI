// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.AI;
using System.Text.Json;

namespace ASOFT.CoreAI.Abstractions;

/// <summary>Provides extension methods for <see cref="StreamingChatMessageContent"/>.</summary>
public static class StreamingChatMessageContentExtensions
{
    /// <summary>Converts a <see cref="StreamingChatMessageContent"/> to a <see cref="ChatResponseUpdate"/>.</summary>
    /// <remarks>This conversion should not be necessary once SK eventually adopts the shared content types.</remarks>
    public static ChatResponseUpdate ToChatResponseUpdate(this StreamingChatMessageContent content)
    {
        ChatResponseUpdate update = new()
        {
            AdditionalProperties = content.Metadata is not null ? new AdditionalPropertiesDictionary(content.Metadata) : null,
            AuthorName = content.AuthorName,
            ModelId = content.ModelId,
            RawRepresentation = content.InnerContent,
            Role = content.Role is not null ? new ChatRole(content.Role.Value.Label) : null,
        };

        foreach (var item in content.Items)
        {
            AIContent? aiContent = null;
            switch (item)
            {
                case StreamingTextContent tc:
                    aiContent = new Microsoft.Extensions.AI.TextContent(tc.Text);
                    break;

                case StreamingFunctionCallUpdateContent fcc:
                    aiContent = new Microsoft.Extensions.AI.FunctionCallContent(
                        fcc.CallId ?? string.Empty,
                        fcc.Name ?? string.Empty,
                        !string.IsNullOrWhiteSpace(fcc.Arguments) ? JsonSerializer.Deserialize<IDictionary<string, object?>>(fcc.Arguments, AbstractionsJsonContext.Default.IDictionaryStringObject!) : null);
                    break;
            }

            if (aiContent is not null)
            {
                aiContent.RawRepresentation = content;

                update.Contents.Add(aiContent);
            }
        }

        return update;
    }

    public static StreamingChatMessageContent ToStreamingChatMessageContent(this ChatResponseUpdate update)
    {
        StreamingChatMessageContent content = new(
            update.Role is not null ? new AuthorRole(update.Role.Value.Value) : null,
            null)
        {
            InnerContent = update.RawRepresentation,
            Metadata = update.AdditionalProperties,
            ModelId = update.ModelId
        };

        //foreach (AIContent item in update.Contents)
        //{
        //    StreamingKernelContent? resultContent =
        //        item is Microsoft.Extensions.AI.TextContent tc ? new Microsoft.SemanticKernel.StreamingTextContent(tc.Text) :
        //        item is Microsoft.Extensions.AI.FunctionCallContent fcc ?
        //            new Microsoft.SemanticKernel.StreamingFunctionCallUpdateContent(fcc.CallId, fcc.Name, fcc.Arguments is not null ?
        //                JsonSerializer.Serialize(fcc.Arguments!, AbstractionsJsonContext.Default.IDictionaryStringObject!) :
        //                null) :
        //        null;

        //    if (resultContent is not null)
        //    {
        //        resultContent.ModelId = update.ModelId;
        //        content.Items.Add(resultContent);
        //    }
        //}

        return content;
    }
}