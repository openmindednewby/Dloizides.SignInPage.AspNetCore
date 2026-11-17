using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class PageSafetyTests
{
    [Fact]
    public void Render_WithAnyModel_LetsPasswordManagersFillTheFields()
    {
        var model = SignInPageSamples.Acme();

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("autocomplete=\"off\"", page);
        Assert.Contains("autocomplete=\"username\"", page);
        Assert.Contains("autocomplete=\"current-password\"", page);
    }

    [Theory]
    [InlineData("//evil.test/font.css")]
    [InlineData("/\\evil.test/font.css")]
    public void Render_WithProtocolRelativeFontUrl_ThrowsArgumentException(string url)
    {
        var model = SignInPageSamples.Acme() with { Shell = SignInPageSamples.Shell() with { FontCssUrl = url } };

        var render = () => SignInPage.Render(model);

        Assert.Throws<ArgumentException>(render);
    }

    [Fact]
    public void Render_WithProtocolRelativeFormAction_ThrowsArgumentException()
    {
        var model = SignInPageSamples.Acme() with { Form = new SignInForm("//evil.test/login") };

        var render = () => SignInPage.Render(model);

        Assert.Throws<ArgumentException>(render);
    }

    [Fact]
    public void Render_WithForgotPasswordOn_LinksToTheRequestPage()
    {
        var config = SignInPageSamples.Config() with
        {
            Features = new SignInFeatures { ForgotPassword = new ForgotPasswordFeature(true, 30) },
            Copy = SignInPageSamples.Copy() with { ForgotPasswordLink = "Forgot password?" },
        };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = config, ForgotPasswordHref = "/forgot-password" });

        Assert.Contains("<a class=\"forgot\" href=\"/forgot-password\">Forgot password?</a>", page);
    }
}
