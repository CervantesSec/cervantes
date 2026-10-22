using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cervantes.IFR.Parsers;

/// <summary>
/// JSON settings shared by the import parsers. Tool output mixes camelCase, PascalCase and snake/kebab case and is not
/// strict about numbers, so names match without case and numbers may come as strings.
/// </summary>
public static class ParserJson
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };
}

/// <summary>Reads a string property that the tool may write as a number or a boolean (e.g. "2" or 2).</summary>
public sealed class FlexibleStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.TryGetInt64(out var whole)
                ? whole.ToString(CultureInfo.InvariantCulture)
                : reader.GetDouble().ToString(CultureInfo.InvariantCulture),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Null => null,
            _ => throw new JsonException($"Unexpected {reader.TokenType} for a text value"),
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}
