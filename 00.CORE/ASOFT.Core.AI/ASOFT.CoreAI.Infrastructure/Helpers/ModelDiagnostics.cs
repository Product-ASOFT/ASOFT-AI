using ASOFT.CoreAI.Abstractions;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ASOFT.CoreAI.Infrastructure;

public static class ModelDiagnostics
{
    private static readonly string s_namespace = typeof(ModelDiagnostics).Namespace!;
    private static readonly ActivitySource s_activitySource = new(s_namespace);

    /// <summary>
    /// Start a chat completion activity for a given model.
    /// The activity will be tagged with the a set of attributes specified by the semantic conventions.
    /// </summary>
    public static Activity? StartCompletionActivity<TPromptExecutionSettings>(
        Uri? endpoint,
        string modelName,
        string modelProvider,
        ChatHistory chatHistory,
        TPromptExecutionSettings? executionSettings) where TPromptExecutionSettings : PromptExecutionSettings
    {
        //if (!IsModelDiagnosticsEnabled())
        //{
        //    return null;
        //}

        const string OperationName = "chat.completions";
        var activity = s_activitySource.StartActivityWithTags(
            $"{OperationName} {modelName}",
            [
                new(ModelDiagnosticsTags.Operation, OperationName),
                new(ModelDiagnosticsTags.System, modelProvider),
                new(ModelDiagnosticsTags.Model, modelName),
            ],
            ActivityKind.Client);

        if (endpoint is not null)
        {
            activity?.SetTags([
                // Skip the query string in the uri as it may contain keys
                new(ModelDiagnosticsTags.Address, endpoint.GetLeftPart(UriPartial.Path)),
                new(ModelDiagnosticsTags.Port, endpoint.Port),
            ]);
        }

        AddOptionalTags(activity, executionSettings);

        //if (s_enableSensitiveEvents)
        //{
        //    foreach (var message in chatHistory)
        //    {
        //        var formattedContent = ToGenAIConventionsFormat(message);
        //        activity?.AttachSensitiveDataAsEvent(
        //            ModelDiagnosticsTags.RoleToEventMap[message.Role],
        //            [
        //                new(ModelDiagnosticsTags.EventName, formattedContent),
        //                new(ModelDiagnosticsTags.System, modelProvider),
        //            ]);
        //    }
        //}

        return activity;
    }

    /// <summary>
    /// Start an agent invocation activity and return the activity.
    /// </summary>
    public static Activity? StartAgentInvocationActivity(
        string agentId,
        string agentName,
        string? agentDescription)
    {
        //if (!IsModelDiagnosticsEnabled())
        //{
        //    return null;
        //}

        const string OperationName = "invoke_agent";

        var activity = s_activitySource.StartActivityWithTags(
            $"{OperationName} {agentName}",
            [
                new(ModelDiagnosticsTags.Operation, OperationName),
                new(ModelDiagnosticsTags.AgentId, agentId),
                new(ModelDiagnosticsTags.AgentName, agentName)
            ],
            ActivityKind.Internal);

        if (!string.IsNullOrWhiteSpace(agentDescription))
        {
            activity?.SetTag(ModelDiagnosticsTags.AgentDescription, agentDescription);
        }

        return activity;
    }

    /// <summary>
    /// Set the chat completion response for a given activity.
    /// The activity will be enriched with the response attributes specified by the semantic conventions.
    /// </summary>
    public static void SetCompletionResponse(this Activity activity, IEnumerable<ChatMessageContent> completions, int? promptTokens = null, int? completionTokens = null)
        => SetCompletionResponse(activity, completions, promptTokens, completionTokens, ToGenAIConventionsChoiceFormat);

    #region Private

    private static void AddOptionalTags<TPromptExecutionSettings>(Activity? activity, TPromptExecutionSettings? executionSettings)
        where TPromptExecutionSettings : PromptExecutionSettings
    {
        if (activity is null || executionSettings is null)
        {
            return;
        }

        // Serialize and deserialize the execution settings to get the extension data
        var deserializedSettings = JsonSerializer.Deserialize<PromptExecutionSettings>(JsonSerializer.Serialize(executionSettings));
        if (deserializedSettings is null || deserializedSettings.ExtensionData is null)
        {
            return;
        }

        void TryAddTag(string key, string tag)
        {
            if (deserializedSettings.ExtensionData.TryGetValue(key, out var value))
            {
                activity.SetTag(tag, value);
            }
        }

        TryAddTag("max_tokens", ModelDiagnosticsTags.MaxToken);
        TryAddTag("temperature", ModelDiagnosticsTags.Temperature);
        TryAddTag("top_p", ModelDiagnosticsTags.TopP);
    }

    /// <summary>
    /// Convert a chat message to a string aligned with the OTel GenAI Semantic Conventions format
    /// </summary>
    private static string ToGenAIConventionsFormat(ChatMessageContent chatMessage, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        sb.Append("{\"role\": \"");
        sb.Append(chatMessage.Role);
        sb.Append("\", \"content\": ");
        sb.Append(JsonSerializer.Serialize(chatMessage.Content));
        if (chatMessage.Items.OfType<FunctionCallContent>().Any())
        {
            sb.Append(", \"tool_calls\": ");
            ToGenAIConventionsFormat(chatMessage.Items, sb);
        }
        sb.Append('}');

        return sb.ToString();
    }

    /// <summary>
    /// Helper method to convert tool calls to a string aligned with the OTel GenAI Semantic Conventions format
    /// </summary>
    private static void ToGenAIConventionsFormat(ChatMessageContentItemCollection chatMessageContentItems, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        sb.Append('[');
        var isFirst = true;
        foreach (var functionCall in chatMessageContentItems.OfType<FunctionCallContent>())
        {
            if (!isFirst)
            {
                // Append a comma and a newline to separate the elements after the previous one.
                // This can avoid adding an unnecessary comma after the last element.
                sb.Append(", \n");
            }

            sb.Append("{\"id\": \"");
            //sb.Append(functionCall.Id);
            sb.Append("\", \"function\": {\"arguments\": ");
            sb.Append(JsonSerializer.Serialize(functionCall.Arguments));
            sb.Append(", \"name\": \"");
            //sb.Append(functionCall.FunctionName);
            sb.Append("\"}, \"type\": \"function\"}");

            isFirst = false;
        }
        sb.Append(']');
    }

    /// <summary>
    /// Convert a chat model response to a string aligned with the OTel GenAI Semantic Conventions format
    /// </summary>
    private static string ToGenAIConventionsChoiceFormat(ChatMessageContent chatMessage, int index)
    {
        var sb = new StringBuilder();

        sb.Append("{\"index\": ");
        sb.Append(index);
        sb.Append(", \"message\": ");
        ToGenAIConventionsFormat(chatMessage, sb);
        sb.Append(", \"tool_calls\": ");
        ToGenAIConventionsFormat(chatMessage.Items, sb);
        if (chatMessage.Metadata?.TryGetValue("FinishReason", out var finishReason) == true)
        {
            sb.Append(", \"finish_reason\": ");
            sb.Append(JsonSerializer.Serialize(finishReason));
        }
        sb.Append('}');

        return sb.ToString();
    }

    /// <summary>
    /// Set the completion response for a given activity.
    /// The `formatCompletions` delegate won't be invoked if events are disabled.
    /// </summary>
    private static void SetCompletionResponse<T>(
        Activity activity,
        IEnumerable<T> completions,
        int? inputTokens,
        int? outputTokens,
        Func<T, int, string> formatCompletion) where T : KernelContent
    {
        //if (!IsModelDiagnosticsEnabled())
        //{
        //    return;
        //}

        if (inputTokens != null)
        {
            activity.SetTag(ModelDiagnosticsTags.InputTokens, inputTokens);
        }

        if (outputTokens != null)
        {
            activity.SetTag(ModelDiagnosticsTags.OutputTokens, outputTokens);
        }

        activity.SetFinishReasons(completions);

        //if (s_enableSensitiveEvents)
        //{
        //    bool responseIdSet = false;
        //    int index = 0;
        //    foreach (var completion in completions)
        //    {
        //        if (!responseIdSet)
        //        {
        //            activity.SetResponseId(completion);
        //            responseIdSet = true;
        //        }

        //        var formattedContent = formatCompletion(completion, index++);
        //        activity.AttachSensitiveDataAsEvent(
        //            ModelDiagnosticsTags.Choice,
        //            [
        //                new(ModelDiagnosticsTags.EventName, formattedContent),
        //            ]);
        //    }
        //}
        //else
        //{
        //    activity.SetResponseId(completions.FirstOrDefault());
        //}
    }

    // Returns an activity for chaining
    private static Activity SetFinishReasons(this Activity activity, IEnumerable<KernelContent> completions)
    {
        var finishReasons = completions.Select(c =>
        {
            if (c.Metadata?.TryGetValue("FinishReason", out var finishReason) == true && !string.IsNullOrEmpty(finishReason as string))
            {
                return finishReason;
            }

            return "N/A";
        });

        if (finishReasons.Any())
        {
            activity.SetTag(ModelDiagnosticsTags.FinishReason, $"[{string.Join(",",
                finishReasons.Select(finishReason => $"\"{finishReason}\""))}]");
        }

        return activity;
    }

    /// <summary>
    /// Tags used in model diagnostics
    /// </summary>
    private static class ModelDiagnosticsTags
    {
        // Activity tags
        public const string System = "gen_ai.system";

        public const string Operation = "gen_ai.operation.name";
        public const string Model = "gen_ai.request.model";
        public const string MaxToken = "gen_ai.request.max_tokens";
        public const string Temperature = "gen_ai.request.temperature";
        public const string TopP = "gen_ai.request.top_p";
        public const string ResponseId = "gen_ai.response.id";
        public const string ResponseModel = "gen_ai.response.model";
        public const string FinishReason = "gen_ai.response.finish_reason";
        public const string InputTokens = "gen_ai.usage.input_tokens";
        public const string OutputTokens = "gen_ai.usage.output_tokens";
        public const string Address = "server.address";
        public const string Port = "server.port";
        public const string AgentId = "gen_ai.agent.id";
        public const string AgentName = "gen_ai.agent.name";
        public const string AgentDescription = "gen_ai.agent.description";

        // Activity events
        public const string EventName = "gen_ai.event.content";

        public const string SystemMessage = "gen_ai.system.message";
        public const string UserMessage = "gen_ai.user.message";
        public const string AssistantMessage = "gen_ai.assistant.message";
        public const string ToolMessage = "gen_ai.tool.message";
        public const string Choice = "gen_ai.choice";

        public static readonly Dictionary<AuthorRole, string> RoleToEventMap = new()
            {
                { AuthorRole.System, SystemMessage },
                { AuthorRole.User, UserMessage },
                { AuthorRole.Assistant, AssistantMessage },
                { AuthorRole.Tool, ToolMessage }
            };
    }

    # endregion
}