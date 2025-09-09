using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASOFT.CoreAI.Infrastructure;

public static class JsonOptionsCache
{
    /// <summary>
    /// Cached <see cref="JsonSerializerOptions"/> instance for reading and writing JSON using the default settings.
    /// </summary>
    public static JsonSerializerOptions Default { get; } = new();

    /// <summary>
    /// Cached <see cref="JsonSerializerOptions"/> instance for writing JSON with indentation.
    /// </summary>
    public static JsonSerializerOptions WriteIndented { get; } = new()
    {
        WriteIndented = true,
    };

    /// <summary>
    /// Cached <see cref="JsonSerializerOptions"/> instance for reading JSON in a permissive way,
    /// including support for trailing commas, case-insensitive property names, and comments.
    /// </summary>
    public static JsonSerializerOptions ReadPermissive { get; } = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/> configured for serializing chat history data.
    /// </summary>
    public static JsonSerializerOptions ChatHistory { get; } = new()
    {
        Converters = { new ExceptionJsonConverter() }
    };
}

internal sealed class ExceptionJsonConverter : JsonConverter<object>
{
    private const string ClassNamePropertyName = "className";
    private const string MessagePropertyName = "message";
    private const string InnerExceptionPropertyName = "innerException";
    private const string StackTracePropertyName = "stackTraceString";

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Exception).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is Exception ex)
        {
            writer.WriteStartObject();
            writer.WriteString(ClassNamePropertyName, ex.GetType().ToString());
            writer.WriteString(MessagePropertyName, ex.Message);
            if (ex.InnerException is Exception innerEx)
            {
                writer.WritePropertyName(InnerExceptionPropertyName);
                this.Write(writer, innerEx, options);
            }

            writer.WriteString(StackTracePropertyName, ex.StackTrace);
            writer.WriteEndObject();
        }
    }

    /// <inheritdoc/>
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}