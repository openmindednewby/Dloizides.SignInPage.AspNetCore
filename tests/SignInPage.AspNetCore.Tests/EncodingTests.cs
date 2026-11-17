using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class EncodingTests
{
    private const string Payload = "<script>alert(1)</script>";

    [Fact]
    public void Render_WithScriptInError_EncodesIt()
    {
        var model = SignInPageSamples.Acme() with { Form = new SignInForm("/login", Error: Payload) };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain(Payload, page);
        Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", page);
    }

    [Fact]
    public void Render_WithScriptInReturnUrl_EncodesIt()
    {
        var model = SignInPageSamples.Acme() with { Form = new SignInForm("/login", ReturnUrl: "\"><script>alert(1)</script>") };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain(Payload, page);
        Assert.Contains("value=\"&quot;&gt;&lt;script&gt;", page);
    }

    [Fact]
    public void Render_WithScriptInLabels_EncodesThem()
    {
        var model = SignInPageSamples.Acme() with
        {
            Copy = SignInPageSamples.Copy() with { UsernameLabel = Payload, SubmitLabel = Payload, Footer = Payload },
        };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain(Payload, page);
        Assert.Contains("&lt;script&gt;", page);
    }

    [Fact]
    public void Render_WithScriptInDemoValues_EncodesThem()
    {
        var model = SignInPageSamples.Acme() with { DemoAccounts = [new SignInDemoAccount(Payload, Payload, Payload)] };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain(Payload, page);
        Assert.Contains("data-password=\"&lt;script&gt;", page);
    }
}
