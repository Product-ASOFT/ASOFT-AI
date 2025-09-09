// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.AI;

namespace ASOFT.CoreAI.Abstractions.ChatClient;

public static class ChatMessageExtensions
{
    /// <summary>Converts a <see cref="ChatMessage"/> to a <see cref="ChatMessageContent"/>.</summary>
    public static ChatMessageContent ToChatMessageContent(this ChatMessage message, ChatResponse? response = null)
    {
        //ChatMessageContent result = new()
        //{
        //    ModelId = response?.ModelId,
        //    AuthorName = message.AuthorName,
        //    InnerContent = response?.RawRepresentation ?? message.RawRepresentation,
        //    Metadata = message.AdditionalProperties,
        //    Role = new AuthorRole(message.Role.Value),
        //};

        //foreach (AIContent content in message.Contents)
        //{
        //    KernelContent? resultContent = content switch
        //    {
        //        Microsoft.Extensions.AI.TextContent tc => new TextContent(tc.Text),
        //        //Microsoft.Extensions.AI.DataContent dc when dc.HasTopLevelMediaType("image") => new Microsoft.SemanticKernel.ImageContent(dc.Uri),
        //        //Microsoft.Extensions.AI.UriContent uc when uc.HasTopLevelMediaType("image") => new Microsoft.SemanticKernel.ImageContent(uc.Uri),

        //        //Microsoft.Extensions.AI.FunctionCallContent fcc => new Microsoft.SemanticKernel.FunctionCallContent(fcc.Name, null, fcc.CallId, fcc.Arguments is not null ? new(fcc.Arguments) : null),
        //        //Microsoft.Extensions.AI.FunctionResultContent frc => new Microsoft.SemanticKernel.FunctionResultContent(callId: frc.CallId, result: frc.Result),
        //        _ => null
        //    };

        //    if (resultContent is not null)
        //    {
        //        resultContent.Metadata = content.AdditionalProperties;
        //        resultContent.InnerContent = content.RawRepresentation;
        //        resultContent.ModelId = response?.ModelId;
        //        result.Items.Add(resultContent);
        //    }
        //}

        return null;
    }

    /// <summary>Converts a list of <see cref="ChatMessage"/> to a <see cref="ChatHistory"/>.</summary>
    internal static ChatHistory ToChatHistory(this IEnumerable<ChatMessage> chatMessages)
    {
        ChatHistory chatHistory = [];
        foreach (var message in chatMessages)
        {
            chatHistory.Add(message.ToChatMessageContent());
        }
        return chatHistory;
    }
}