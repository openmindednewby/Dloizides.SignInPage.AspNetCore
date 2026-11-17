using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class BrandOptionalTests
{
    [Fact]
    public void Render_WithoutBrand_HasNoBrandElement()
    {
        var model = SignInPageSamples.Acme() with { Shell = new SignInPageShell("Sign in", null) };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("<div class=\"brand\">", page);
        Assert.DoesNotContain("class=\"brand-name\"", page);
    }

    [Fact]
    public void Render_WithEmptyBrandName_HasNoBrandElementOrTagline()
    {
        var model = SignInPageSamples.Acme() with { Shell = new SignInPageShell("Sign in", new SignInBrand(" ", "Cloud", "Identity")) };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("<div class=\"brand\">", page);
        Assert.DoesNotContain("<span>Identity</span>", page);
    }

    [Fact]
    public void Render_WithBrandName_ShowsTheBrandElement()
    {
        var model = SignInPageSamples.Acme() with { Shell = new SignInPageShell("Sign in", new SignInBrand("Acme")) };

        var page = SignInPage.Render(model);

        Assert.Contains("<div class=\"brand\">", page);
        Assert.Contains("<span class=\"brand-name\">Acme</span>", page);
    }

    [Fact]
    public void ResetRequestPage_WithoutBrand_TitlesThePageWithoutABrandSuffix()
    {
        var config = PasswordResetSamples.Config(30, 8);
        var shell = new SignInPageShell("Sign in", null);

        var page = PasswordResetPages.Request(shell, config, PasswordResetSamples.Paths("https://id.acme.test"));

        Assert.Contains($"<title>{config.Copy.ForgotPasswordTitle}</title>", page);
        Assert.DoesNotContain("<div class=\"brand\">", page);
    }
}
