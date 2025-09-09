// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.AI;

namespace ASOFT.CoreAI.Abstractions.ChatClient;

public static class ChatResponseUpdateExtensions
{
    internal static StreamingChatMessageContent ToStreamingChatMessageContent(this ChatResponseUpdate update)
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
        //        item is Microsoft.Extensions.AI.TextContent tc ? new StreamingTextContent(tc.Text) :
        //        item is FunctionCallContent fcc ?
        //            new StreamingFunctionCallUpdateContent(fcc.CallId, fcc.PluginName, fcc.Arguments is not null ?
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