using System.Text.Json;

namespace SignInPage.AspNetCore;

/// <summary>The embedded JSON Schema, byte-identical to the @dloizides/sign-in-config 1.0.0 schema.</summary>
public static class SignInConfigSchema
{
    private static readonly Lazy<string> Text = new(ReadEmbedded);

    private static readonly Lazy<JsonElement> Parsed = new(() => JsonDocument.Parse(Text.Value).RootElement.Clone());

    public static string Json => Text.Value;

    internal static JsonElement Root => Parsed.Value;

    private static string ReadEmbedded()
    {
        using var stream = typeof(SignInConfigSchema).Assembly.GetManifestResourceStream("SignInPage.AspNetCore.sign-in-config.schema.json")
            ?? throw new InvalidOperationException("The sign-in config schema is not embedded.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
