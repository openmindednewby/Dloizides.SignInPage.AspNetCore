using System.Text.RegularExpressions;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class ThemeTests
{
    [Fact]
    public void Render_WithChangedBrandToken_EmitsTheNewColour()
    {
        var theme = SignInPageSamples.Theme() with { Colors = SignInPageSamples.Colors() with { Brand = "#123456" } };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = SignInPageSamples.Config() with { Theme = theme } });

        Assert.Contains("--brand:#123456;", page);
    }

    [Fact]
    public void Render_WithSuccessToken_EmitsItAsAVariable()
    {
        var theme = SignInPageSamples.Theme() with { Colors = SignInPageSamples.Colors() with { Success = "#1f7a3d" } };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = SignInPageSamples.Config() with { Theme = theme } });

        Assert.Contains("--success:#1f7a3d;", page);
    }

    [Fact]
    public void Render_WithDemoAccountsAndErrors_PaintsNoColourOutsideTheThemeVariables()
    {
        var copy = SignInPageSamples.Copy() with { DemoToggle = "Demo", DemoHint = "Hint", DemoAriaLabel = "Demo", DemoRowAriaLabel = "Sign in as {label}" };
        var model = SignInPageSamples.Acme() with
        {
            Config = SignInPageSamples.Config() with { Copy = copy, Features = new SignInFeatures { DemoAccounts = new DemoAccountsFeature(true) } },
            Form = new SignInForm("/login", "/back", "Wrong password"),
            FieldErrors = new Dictionary<string, string> { ["username"] = "Email is required" },
            DemoAccounts = [new SignInDemoAccount("Admin", "admin@acme.test", "pw")],
        };

        var page = SignInPage.Render(model);

        var withoutRoot = Regex.Replace(page, @":root\s*\{[^}]*\}", string.Empty);
        Assert.DoesNotMatch(@"#[0-9a-fA-F]{3,8}\b", withoutRoot);
        Assert.DoesNotMatch(@"\b(rgb|rgba|hsl|hsla)\(", withoutRoot);
        Assert.Contains("var(--brand)", page);
    }

    [Theory]
    [InlineData("#fff;} body{display:none")]
    [InlineData("red</style><script>alert(1)</script>")]
    public void Render_WithStyleBreakingToken_ThrowsArgumentException(string token)
    {
        var theme = SignInPageSamples.Theme() with { Colors = SignInPageSamples.Colors() with { Ink = token } };
        var model = SignInPageSamples.Acme() with { Config = SignInPageSamples.Config() with { Theme = theme } };

        var render = () => SignInPage.Render(model);

        Assert.Throws<ArgumentException>(render);
    }
}
