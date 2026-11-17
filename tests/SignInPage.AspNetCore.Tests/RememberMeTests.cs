using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class RememberMeTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 17, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Properties_WhenUnchecked_ReturnsASessionCookie()
    {
        var config = SignInPageSamples.Config() with { Features = new SignInFeatures { RememberMe = new RememberMeFeature(true, 720) } };

        var properties = RememberMe.Properties(config, false, Now);

        Assert.False(properties.IsPersistent);
        Assert.Null(properties.ExpiresUtc);
    }

    [Fact]
    public void Properties_WhenChecked_PersistsForTheConfiguredHours()
    {
        var config = SignInPageSamples.Config() with { Features = new SignInFeatures { RememberMe = new RememberMeFeature(true, 720) } };

        var properties = RememberMe.Properties(config, true, Now);

        Assert.True(properties.IsPersistent);
        Assert.Equal(Now.AddHours(720), properties.ExpiresUtc);
    }

    [Fact]
    public void Properties_WhenCheckedWithLowerSessionLimit_CapsAtTheSessionLimit()
    {
        var config = SignInPageSamples.Config() with { Features = new SignInFeatures { RememberMe = new RememberMeFeature(true, 720) } };

        var properties = RememberMe.Properties(config, true, Now, sessionMaxHours: 10);

        Assert.Equal(Now.AddHours(10), properties.ExpiresUtc);
    }

    [Fact]
    public void Properties_WhenCheckedWithFeatureOff_ReturnsASessionCookie()
    {
        var config = SignInPageSamples.Config() with { Features = new SignInFeatures { RememberMe = new RememberMeFeature(false, null) } };

        var properties = RememberMe.Properties(config, true, Now);

        Assert.False(properties.IsPersistent);
    }

    [Fact]
    public void Render_WithRememberMeOn_ShowsTheCheckbox()
    {
        var config = SignInPageSamples.Config() with
        {
            Features = new SignInFeatures { RememberMe = new RememberMeFeature(true, 720) },
            Copy = SignInPageSamples.Copy() with { RememberMeLabel = "Keep me signed in" },
        };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = config });

        Assert.Contains("<input id=\"rememberMe\" name=\"rememberMe\" type=\"checkbox\" value=\"true\" />", page);
        Assert.Contains("Keep me signed in", page);
    }

    [Fact]
    public void Render_WithoutRememberMe_HasNoCheckbox()
    {
        var model = SignInPageSamples.Acme();

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("rememberMe", page);
    }
}
