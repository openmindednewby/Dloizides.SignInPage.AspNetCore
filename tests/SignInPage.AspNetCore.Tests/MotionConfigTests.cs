using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class MotionConfigTests
{
    [Fact]
    public void Render_WithMotionEnabled_EmitsTheConfiguredPresets()
    {
        var motion = new SignInMotion
        {
            Enabled = true,
            Intro = new SignInMotionStep("stagger-rise", 600),
            Micro = new SignInMotionStep("sweep", 300),
            Idle = new SignInIdleMotion("breath", null),
        };
        var model = SignInPageSamples.Acme() with
        {
            Config = SignInPageSamples.Config() with { Motion = motion },
            Shell = SignInPageSamples.Shell() with { Logo = new SignInLogo("/acme-logo.webp", "Acme", "200px") },
        };

        var page = SignInPage.Render(model);

        Assert.Contains("data-intro=\"stagger-rise\" data-intro-ms=\"600\" data-micro=\"sweep\" data-micro-ms=\"300\" data-idle=\"breath\"", page);
    }

    [Fact]
    public void Render_WithMotionDisabled_EmitsTheStillLogoOnly()
    {
        var motion = new SignInMotion { Enabled = false, Intro = new SignInMotionStep("stagger-rise", 600) };
        var model = SignInPageSamples.Acme() with
        {
            Config = SignInPageSamples.Config() with { Motion = motion },
            Shell = SignInPageSamples.Shell() with { Logo = new SignInLogo("/acme-logo.webp", "Acme", "200px") },
        };

        var page = SignInPage.Render(model);

        Assert.Contains("<img src=\"/acme-logo.webp\" alt=\"Acme\"", page);
        Assert.DoesNotContain("data-intro", page);
    }

    [Fact]
    public void Render_WithoutLogo_HasNoLogoSlot()
    {
        var model = SignInPageSamples.Acme() with { Shell = SignInPageSamples.Shell() with { Logo = null } };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("id=\"login-logo\"", page);
    }

    [Fact]
    public void Render_WithAnyModel_LoadsTheEntryScriptAndSubmitsOnEnter()
    {
        var model = SignInPageSamples.Acme() with
        {
            Form = new SignInForm("/login"),
            Config = SignInPageSamples.Config() with { Copy = SignInPageSamples.Copy() with { SubmitLabel = "Sign in" } },
        };

        var page = SignInPage.Render(model);

        Assert.Contains("<script type=\"module\" src=\"/_content/Dloizides.SignInPage.AspNetCore/login/sign-in.js\"></script>", page);
        Assert.Matches("<form class=\"card\" method=\"post\" action=\"/login\"[^>]*>[\\s\\S]*<button type=\"submit\">Sign in</button>[\\s\\S]*</form>", page);
    }
}
