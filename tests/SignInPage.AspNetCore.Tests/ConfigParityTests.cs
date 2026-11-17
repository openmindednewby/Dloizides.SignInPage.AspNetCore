using System.Security.Cryptography;
using System.Text;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class ConfigParityTests
{
    public static TheoryData<string> ValidFiles() => SharedFixtures.Files("valid");

    public static TheoryData<string> InvalidFiles() => SharedFixtures.Files("invalid");

    [Theory]
    [MemberData(nameof(ValidFiles))]
    public void Parse_WithValidSharedFixture_ReturnsTheConfig(string file)
    {
        var json = SharedFixtures.Json(file);
        var options = SharedFixtures.Options("valid", file);

        var result = SignInConfigParser.Parse(json, options);

        Assert.Empty(result.Errors);
        Assert.NotNull(result.Config);
    }

    [Theory]
    [MemberData(nameof(InvalidFiles))]
    public void Parse_WithInvalidSharedFixture_ReturnsTheManifestErrors(string file)
    {
        var json = SharedFixtures.Json(file);
        var options = SharedFixtures.Options("invalid", file);
        var expected = SharedFixtures.ExpectedErrors(file);

        var result = SignInConfigParser.Parse(json, options);

        Assert.Equal(expected, result.Errors.Select(error => $"{error.Path}:{error.Keyword}"));
        Assert.Null(result.Config);
    }

    [Fact]
    public void Schema_WhenEmbedded_IsByteForByteSignInConfigSchema100()
    {
        const string signInConfig100SchemaSha256 = "22ca8cbaa60bd2f05d081cc33b7211dc6b12ce2965e610cd3bfd5b303fa82345";

        var actual = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(SignInConfigSchema.Json)));

        Assert.Equal(signInConfig100SchemaSha256, actual);
    }
}
