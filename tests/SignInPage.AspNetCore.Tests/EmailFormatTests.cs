using System.Text.RegularExpressions;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class EmailFormatTests
{
    [Fact]
    public void Render_WithNoUsernameRule_EmitsTheConfiguredEmailFormatMessage()
    {
        var config = SignInPageSamples.Config() with { Copy = SignInPageSamples.Copy() with { EmailInvalidMessage = "Enter a full email address" } };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = config });

        var input = Regex.Match(page, "<input id=\"email\"[^>]*>").Value;
        Assert.Contains("data-kind=\"email\" data-msg-email=\"Enter a full email address\"", input);
    }

    [Fact]
    public void Render_WithTextUsernameRule_EmitsNoEmailCheck()
    {
        var config = SignInPageSamples.Config() with { Validation = new SignInValidation { Username = new SignInFieldRule { Kind = "text" } } };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = config });

        Assert.DoesNotContain("data-kind=\"email\"", page);
    }

    [Fact]
    public void Validate_WithNoUsernameRuleAndPlainText_ReturnsTheEmailFormatMessage()
    {
        var config = SignInPageSamples.Config() with { Copy = SignInPageSamples.Copy() with { EmailInvalidMessage = "Enter a valid email address" } };

        var errors = SignInFieldValidator.Validate(config, "not-an-email", "pw");

        Assert.Equal("Enter a valid email address", errors["username"]);
    }

    [Fact]
    public void ForgotPage_WithNoUsernameRule_EmitsSeparateRequiredAndFormatMessages()
    {
        var config = PasswordResetSamples.Config(30, 8) with { Copy = PasswordResetSamples.Config(30, 8).Copy with { EmailInvalidMessage = "Enter a valid email address" } };
        var flow = new PasswordResetFlow(null!, config, SignInPageSamples.Shell(), PasswordResetSamples.Paths("https://id.acme.test"));

        var page = flow.GetForgot();

        Assert.Contains("data-required=\"true\" data-msg-required=\"Email is required\"", page);
        Assert.Contains("data-kind=\"email\" data-msg-email=\"Enter a valid email address\"", page);
    }

    [Fact]
    public void EmailMessage_WithNoConfiguredFormatMessage_NeverReusesTheRequiredMessage()
    {
        var config = SignInPageSamples.Config() with { Validation = new SignInValidation { Username = new SignInFieldRule { Kind = "email" } } };

        var errors = SignInFieldValidator.Validate(config, "not-an-email", "pw");

        Assert.NotEqual("Email is required", errors["username"]);
        Assert.Contains("Email", errors["username"], StringComparison.Ordinal);
    }

    [Fact]
    public void Render_WithDefaultStyles_GivesPageLinksA44By44HitBox()
    {
        var page = SignInPage.Render(SignInPageSamples.Acme());

        Assert.Contains(".links a { color:var(--brand-on-light); min-height:44px; min-width:44px;", page);
    }
}
