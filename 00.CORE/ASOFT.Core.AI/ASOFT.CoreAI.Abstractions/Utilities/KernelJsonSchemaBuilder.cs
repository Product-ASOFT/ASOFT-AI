using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ASOFT.CoreAI.Abstractions.Utilities;

public static class KernelJsonSchemaBuilder
{
    private static JsonSerializerOptions? s_options;
    internal static readonly AIJsonSchemaCreateOptions s_schemaOptions = new();

    private static readonly JsonElement s_trueSchemaAsObject = JsonDocument.Parse("{}").RootElement;
    private static readonly JsonElement s_falseSchemaAsObject = JsonDocument.Parse("""{"not":true}""").RootElement;

    [RequiresUnreferencedCode("Uses reflection to generate JSON schema, making it incompatible with AOT scenarios.")]
    [RequiresDynamicCode("Uses reflection to generate JSON schema, making it incompatible with AOT scenarios.")]
    public static KernelJsonSchema Build(Type type, string? description = null, AIJsonSchemaCreateOptions? configuration = null)
    {
        return Build(type, GetDefaultOptions(), description, configuration);
    }

    public static KernelJsonSchema Build(
        Type type,
        JsonSerializerOptions options,
        string? description = null,
        AIJsonSchemaCreateOptions? configuration = null)
    {
        configuration ??= s_schemaOptions;
        // To be compatible with the previous behavior of MEAI 9.3.0 (when description is empty, should not be included in the schema)
        string? schemaDescription = string.IsNullOrEmpty(description) ? null : description;
        JsonElement schemaDocument = AIJsonUtilities.CreateJsonSchema(type, schemaDescription, serializerOptions: options, inferenceOptions: configuration);
        switch (schemaDocument.ValueKind)
        {
            case JsonValueKind.False:
                schemaDocument = s_falseSchemaAsObject;
                break;

            case JsonValueKind.True:
                schemaDocument = s_trueSchemaAsObject;
                break;
        }

        return null;
    }

    [RequiresUnreferencedCode("Uses JsonStringEnumConverter and DefaultJsonTypeInfoResolver classes, making it incompatible with AOT scenarios.")]
    [RequiresDynamicCode("Uses JsonStringEnumConverter and DefaultJsonTypeInfoResolver classes, making it incompatible with AOT scenarios.")]
    private static JsonSerializerOptions GetDefaultOptions()
    {
        if (s_options is null)
        {
            JsonSerializerOptions options = new()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Converters = { new JsonStringEnumConverter() },
            };
            options.MakeReadOnly();
            s_options = options;
        }

        return s_options;
    }
}