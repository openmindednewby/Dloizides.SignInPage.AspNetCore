using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SignInPage.AspNetCore;

internal static class SchemaValidator
{
    private const string DefsPrefix = "#/$defs/";

    public static List<SignInConfigError> Validate(JsonElement root, JsonElement value)
    {
        var sink = new List<SignInConfigError>();
        Visit(root, root, value, string.Empty, sink);
        return sink;
    }

    private static void Visit(JsonElement root, JsonElement node, JsonElement value, string path, List<SignInConfigError> sink)
    {
        var schema = Resolve(root, node);
        if (!CheckType(schema, value, path, sink))
            return;
        CheckValue(schema, value, path, sink);
        if (value.ValueKind == JsonValueKind.Object)
            CheckObject(root, schema, value, path, sink);
        if (schema.TryGetProperty("allOf", out var parts))
        {
            foreach (var part in parts.EnumerateArray())
                Visit(root, part, value, path, sink);
        }

        if (schema.TryGetProperty("if", out var condition) && schema.TryGetProperty("then", out var then))
        {
            var probe = new List<SignInConfigError>();
            Visit(root, condition, value, path, probe);
            if (probe.Count == 0)
                Visit(root, then, value, path, sink);
        }
    }

    private static JsonElement Resolve(JsonElement root, JsonElement node) =>
        node.TryGetProperty("$ref", out var reference) && reference.GetString() is { } target
            ? root.GetProperty("$defs").GetProperty(target[DefsPrefix.Length..])
            : node;

    private static bool CheckType(JsonElement schema, JsonElement value, string path, List<SignInConfigError> sink)
    {
        if (!schema.TryGetProperty("type", out var typeNode) || typeNode.GetString() is not { } type || Matches(type, value))
            return true;
        sink.Add(new SignInConfigError(path, "type", $"must be {type}"));
        return false;
    }

    private static bool Matches(string type, JsonElement value) => type switch
    {
        "object" => value.ValueKind == JsonValueKind.Object,
        "integer" => value.ValueKind == JsonValueKind.Number && Math.Floor(value.GetDouble()) == value.GetDouble(),
        "number" => value.ValueKind == JsonValueKind.Number,
        "string" => value.ValueKind == JsonValueKind.String,
        "boolean" => value.ValueKind is JsonValueKind.True or JsonValueKind.False,
        _ => false,
    };

    private static void CheckValue(JsonElement schema, JsonElement value, string path, List<SignInConfigError> sink)
    {
        if (schema.TryGetProperty("const", out var constant) && !JsonEquals(value, constant))
            sink.Add(new SignInConfigError(path, "const", $"must equal {constant.GetRawText()}"));
        if (schema.TryGetProperty("enum", out var options) && !options.EnumerateArray().Any(option => JsonEquals(value, option)))
            sink.Add(new SignInConfigError(path, "enum", $"must be one of {string.Join(", ", options.EnumerateArray().Select(option => option.ToString()))}"));
        if (value.ValueKind == JsonValueKind.String)
            CheckString(schema, value.GetString()!, path, sink);
        if (value.ValueKind == JsonValueKind.Number)
            CheckNumber(schema, value.GetDouble(), path, sink);
    }

    private static void CheckString(JsonElement schema, string value, string path, List<SignInConfigError> sink)
    {
        var length = value.EnumerateRunes().Count();
        if (NumberOf(schema, "minLength") is { } min && length < min)
            sink.Add(new SignInConfigError(path, "minLength", Text($"must have at least {min} characters")));
        if (NumberOf(schema, "maxLength") is { } max && length > max)
            sink.Add(new SignInConfigError(path, "maxLength", Text($"must have at most {max} characters")));
        if (schema.TryGetProperty("pattern", out var pattern) && !Regex.IsMatch(value, EndAnchored(pattern.GetString()!), RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1)))
            sink.Add(new SignInConfigError(path, "pattern", $"must match {pattern.GetString()}"));
    }

    private static void CheckNumber(JsonElement schema, double value, string path, List<SignInConfigError> sink)
    {
        if (NumberOf(schema, "minimum") is { } min && value < min)
            sink.Add(new SignInConfigError(path, "minimum", Text($"must be at least {min}")));
        if (NumberOf(schema, "maximum") is { } max && value > max)
            sink.Add(new SignInConfigError(path, "maximum", Text($"must be at most {max}")));
        if (NumberOf(schema, "exclusiveMinimum") is { } above && value <= above)
            sink.Add(new SignInConfigError(path, "exclusiveMinimum", Text($"must be more than {above}")));
    }

    private static void CheckObject(JsonElement root, JsonElement schema, JsonElement value, string path, List<SignInConfigError> sink)
    {
        if (schema.TryGetProperty("required", out var required))
        {
            foreach (var key in required.EnumerateArray().Select(key => key.GetString()!).Where(key => !value.TryGetProperty(key, out _)))
                sink.Add(new SignInConfigError(Child(path, key), "required", "is required"));
        }

        if (schema.TryGetProperty("dependentRequired", out var dependent))
        {
            foreach (var trigger in dependent.EnumerateObject().Where(trigger => value.TryGetProperty(trigger.Name, out _)))
            {
                foreach (var key in trigger.Value.EnumerateArray().Select(key => key.GetString()!).Where(key => !value.TryGetProperty(key, out _)))
                    sink.Add(new SignInConfigError(Child(path, key), "dependentRequired", $"is required when {trigger.Name} is set"));
            }
        }

        var hasProperties = schema.TryGetProperty("properties", out var properties);
        foreach (var member in value.EnumerateObject())
        {
            if (hasProperties && properties.TryGetProperty(member.Name, out var sub))
                Visit(root, sub, member.Value, Child(path, member.Name), sink);
            else if (schema.TryGetProperty("additionalProperties", out var additional) && additional.ValueKind == JsonValueKind.False)
                sink.Add(new SignInConfigError(Child(path, member.Name), "additionalProperties", "is not an allowed key"));
        }
    }

    private static bool JsonEquals(JsonElement left, JsonElement right) => (left.ValueKind, right.ValueKind) switch
    {
        (JsonValueKind.Number, JsonValueKind.Number) => left.GetDouble() == right.GetDouble(),
        (JsonValueKind.String, JsonValueKind.String) => left.GetString() == right.GetString(),
        _ => left.ValueKind == right.ValueKind && left.ValueKind is JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null,
    };

    private static double? NumberOf(JsonElement schema, string key) =>
        schema.TryGetProperty(key, out var node) && node.ValueKind == JsonValueKind.Number ? node.GetDouble() : null;

    private static string EndAnchored(string pattern) =>
        pattern.EndsWith('$') && !pattern.EndsWith("\\$", StringComparison.Ordinal) ? pattern[..^1] + @"\z" : pattern;

    private static string Child(string path, string key) =>
        $"{path}/{key.Replace("~", "~0", StringComparison.Ordinal).Replace("/", "~1", StringComparison.Ordinal)}";

    private static string Text(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
