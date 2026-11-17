using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class ConfigLoadTests
{
    [Fact]
    public void Load_WithUnknownKey_ThrowsNamingTheFieldPath()
    {
        var path = TempFile("""{"version":1,"colour":"red"}""");

        var load = () => SignInConfigParser.Load(path);

        var error = Assert.Throws<InvalidSignInConfigException>(load);
        Assert.Contains("/colour", error.Message);
    }

    [Fact]
    public void Load_WithMicroMotionOverCap_ThrowsNamingTheFieldPath()
    {
        var path = Path.Combine(SharedFixtures.Root(), "invalid", "motion-micro-over-cap.json");

        var load = () => SignInConfigParser.Load(path);

        var error = Assert.Throws<InvalidSignInConfigException>(load);
        Assert.Contains("/motion/micro/durationMs", error.Message);
    }

    [Fact]
    public void AddSignInConfig_WithInvalidFile_ThrowsAtRegistration()
    {
        var path = TempFile("""{"version":2}""");
        var services = new ServiceCollection();

        var add = () => services.AddSignInConfig(path);

        Assert.Throws<InvalidSignInConfigException>(add);
    }

    [Fact]
    public void AddSignInConfig_WithValidFile_RegistersTheParsedConfig()
    {
        var path = Path.Combine(SharedFixtures.Root(), "valid", "full.json");
        var expected = JsonNode.Parse(File.ReadAllText(path))!["copy"]!["submitLabel"]!.GetValue<string>();
        var services = new ServiceCollection();

        services.AddSignInConfig(path);

        Assert.Equal(expected, services.BuildServiceProvider().GetRequiredService<SignInConfig>().Copy.SubmitLabel);
    }

    [Fact]
    public void Parse_WithProtocolRelativeSignUpHref_ReturnsAUrlError()
    {
        var json = SharedFixtures.Json("valid/full.json").Replace("\"href\": \"/register\"", "\"href\": \"//evil.test/register\"", StringComparison.Ordinal);

        var result = SignInConfigParser.Parse(json);

        Assert.Equal(["/features/signUp/href:url"], result.Errors.Select(error => $"{error.Path}:{error.Keyword}"));
    }

    private static string TempFile(string json)
    {
        var path = Path.Combine(Path.GetTempPath(), $"sign-in-{Guid.NewGuid():N}.json");
        File.WriteAllText(path, json);
        return path;
    }
}
