using System.Text.RegularExpressions;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class ThemeTests
{
    [Fact]
    public void Render_WithChangedBrandToken_EmitsTheNewColour()
    {
        var model = SignInPageSamples.Acme() with { Theme = SignInPageSamples.Theme() with { Brand = "#123456" } };

        var page = SignInPage.Render(model);

        Assert.Contains("--brand:#123456;", page);
    }

    [Fact]
    public void Render_WithDemoAccountsAndError_PaintsNoColourOutsideTheThemeVariables()
    {
        var model = SignInPageSamples.Acme() with
        {
            Form = new SignInForm("/login", "/back", "Wrong password"),
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
        var model = SignInPageSamples.Acme() with { Theme = SignInPageSamples.Theme() with { Ink = token } };

        var render = () => SignInPage.Render(model);

        Assert.Throws<ArgumentException>(render);
    }
}
