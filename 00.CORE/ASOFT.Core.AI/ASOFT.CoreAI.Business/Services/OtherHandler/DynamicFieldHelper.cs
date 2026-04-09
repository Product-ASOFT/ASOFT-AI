using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace ASOFT.CoreAI.Business;
public static class DynamicFieldHelper
{
    public static T? GetValue<T>(IDictionary<string, JToken>? dynamicFields, string fieldName)
    {
        if (dynamicFields == null || string.IsNullOrWhiteSpace(fieldName))
            return default;

        if (!dynamicFields.TryGetValue(fieldName, out var token) || token == null || token.Type == JTokenType.Null)
            return default;

        try
        {
            return token.Value<T>();
        }
        catch
        {
            return default;
        }
    }

    public static T GetValueOrDefault<T>(IDictionary<string, JToken>? dynamicFields, string fieldName, T defaultValue)
    {
        var value = GetValue<T>(dynamicFields, fieldName);
        return value == null ? defaultValue : value;
    }

    public static string GetString(IDictionary<string, JToken>? dynamicFields, string fieldName, string defaultValue = "")
    {
        return GetValue<string>(dynamicFields, fieldName) ?? defaultValue;
    }

    public static int GetInt(IDictionary<string, JToken>? dynamicFields, string fieldName, int defaultValue = 0)
    {
        return GetValue<int?>(dynamicFields, fieldName) ?? defaultValue;
    }

    public static decimal? GetDecimalNullable(IDictionary<string, JToken>? dynamicFields, string fieldName)
    {
        return GetValue<decimal?>(dynamicFields, fieldName);
    }

    public static bool GetBool(IDictionary<string, JToken>? dynamicFields, string fieldName, bool defaultValue = false)
    {
        return GetValue<bool?>(dynamicFields, fieldName) ?? defaultValue;
    }

    public static DateTime? GetDateTimeNullable(IDictionary<string, JToken>? dynamicFields, string fieldName)
    {
        return GetValue<DateTime?>(dynamicFields, fieldName);
    }

}