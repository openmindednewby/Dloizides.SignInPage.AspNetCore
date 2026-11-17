using System.Text.Json;

namespace SignInPage.AspNetCore;

/// <summary>Validates sign-in.json: schema, colours, contrast, patterns, URLs and the session cap.</summary>
public static class SignInConfigParser
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static SignInConfigResult Parse(string json, SignInConfigParseOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(json);
        JsonElement root;
        try
        {
            using var document = JsonDocument.Parse(json);
            root = document.RootElement.Clone();
        }
        catch (JsonException error)
        {
            return Failed([new SignInConfigError(string.Empty, "json", error.Message)]);
        }

        var schemaErrors = SchemaValidator.Validate(SignInConfigSchema.Root, root);
        if (schemaErrors.Count > 0)
            return Failed(schemaErrors);

        var config = root.Deserialize<SignInConfig>(JsonOptions)!;
        var errors = ContrastCheck.Check(config.Theme.Colors)
            .Concat(ConfigRules.CheckValidation(config.Validation))
            .Concat(ConfigRules.CheckSessionCap(config.Features, options ?? new SignInConfigParseOptions()))
            .Concat(ConfigRules.CheckLinks(config.Features))
            .ToList();
        return errors.Count > 0 ? Failed(errors) : new SignInConfigResult(config, []);
    }

    public static SignInConfig Load(string path, SignInConfigParseOptions? options = null)
    {
        var result = Parse(File.ReadAllText(path), options);
        return result.Config ?? throw new InvalidSignInConfigException(path, result.Errors);
    }

    private static SignInConfigResult Failed(IEnumerable<SignInConfigError> errors) =>
        new(null, errors.OrderBy(error => error.Path, StringComparer.Ordinal).ThenBy(error => error.Keyword, StringComparer.Ordinal).ToList());
}
