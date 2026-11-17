using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Time.Testing;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class PasswordResetTests
{
    private static readonly Uri ResetPage = new("https://id.acme.test/reset-password");

    [Fact]
    public async Task RequestAsync_WithKnownEmail_SendsOneLinkAndStoresOnlyTheTokenHash()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var service = new PasswordResetService(host, ResetConfig(30), new FakeTimeProvider());

        await service.RequestAsync("ana@acme.test", ResetPage, CancellationToken.None);

        var link = Assert.Single(host.SentLinks);
        Assert.StartsWith("https://id.acme.test/reset-password?token=", link.AbsoluteUri, StringComparison.Ordinal);
        Assert.DoesNotContain(TokenOf(link), host.Tokens.Keys);
        Assert.Equal("u1", Assert.Single(host.Tokens.Values).UserId);
    }

    [Fact]
    public async Task RequestAsync_WithUnknownEmail_SendsNothingAndStoresNothing()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var service = new PasswordResetService(host, ResetConfig(30), new FakeTimeProvider());

        await service.RequestAsync("nobody@acme.test", ResetPage, CancellationToken.None);

        Assert.Empty(host.SentLinks);
        Assert.Empty(host.Tokens);
    }

    [Fact]
    public async Task PostForgotAsync_WithKnownAndUnknownEmail_AnswersTheSamePage()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var config = ResetConfig(30) with { Copy = ResetConfig(30).Copy with { ForgotPasswordSent = "If an account exists, a link is on its way." } };
        var flow = new PasswordResetFlow(new PasswordResetService(host, config, new FakeTimeProvider()), config, SignInPageSamples.Shell());
        var known = await flow.PostForgotAsync("ana@acme.test", ResetPage, CancellationToken.None);

        var unknown = await flow.PostForgotAsync("nobody@acme.test", ResetPage, CancellationToken.None);

        Assert.Equal(known, unknown);
        Assert.Contains("If an account exists, a link is on its way.", known);
    }

    [Fact]
    public async Task ResetAsync_WithFreshToken_SetsThePassword()
    {
        var (host, service) = await RequestedFor("ana@acme.test", "u1", new FakeTimeProvider());

        var result = await service.ResetAsync(TokenOf(host.SentLinks[0]), "newpass123", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.Done, result.Status);
        Assert.Equal("newpass123", host.Passwords["u1"]);
    }

    [Fact]
    public async Task ResetAsync_WithUsedToken_ReturnsInvalidToken()
    {
        var (host, service) = await RequestedFor("ana@acme.test", "u1", new FakeTimeProvider());
        var token = TokenOf(host.SentLinks[0]);
        await service.ResetAsync(token, "newpass123", CancellationToken.None);

        var second = await service.ResetAsync(token, "otherpass456", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.InvalidToken, second.Status);
        Assert.Equal("newpass123", host.Passwords["u1"]);
    }

    [Fact]
    public async Task ResetAsync_WhenTokenExpired_ReturnsInvalidToken()
    {
        var time = new FakeTimeProvider();
        var (host, service) = await RequestedFor("ana@acme.test", "u1", time);
        time.Advance(TimeSpan.FromMinutes(31));

        var result = await service.ResetAsync(TokenOf(host.SentLinks[0]), "newpass123", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.InvalidToken, result.Status);
        Assert.Empty(host.Passwords);
    }

    [Fact]
    public async Task ResetAsync_WithPasswordFailingRules_KeepsTheTokenUsable()
    {
        var (host, service) = await RequestedFor("ana@acme.test", "u1", new FakeTimeProvider());
        var token = TokenOf(host.SentLinks[0]);

        var rejected = await service.ResetAsync(token, "short", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.InvalidPassword, rejected.Status);
        Assert.Equal("Password needs at least 8 characters", rejected.FieldErrors["password"]);
        Assert.Equal(PasswordResetStatus.Done, (await service.ResetAsync(token, "longenough1", CancellationToken.None)).Status);
    }

    private static async Task<(FakeResetHost Host, PasswordResetService Service)> RequestedFor(string email, string userId, TimeProvider time)
    {
        var host = new FakeResetHost { Users = { [email] = userId } };
        var service = new PasswordResetService(host, ResetConfig(30), time);
        await service.RequestAsync(email, ResetPage, CancellationToken.None);
        return (host, service);
    }

    private static SignInConfig ResetConfig(int tokenMinutes) => SignInPageSamples.Config() with
    {
        Features = new SignInFeatures { ForgotPassword = new ForgotPasswordFeature(true, tokenMinutes) },
        Validation = new SignInValidation { Password = new SignInFieldRule { MinLength = 8 } },
        Copy = SignInPageSamples.Copy() with
        {
            PasswordLabel = "Password",
            TooShortMessage = "{field} needs at least {min} characters",
            ForgotPasswordLink = "Forgot password?",
            ForgotPasswordTitle = "Reset your password",
            ForgotPasswordSubmit = "Send reset link",
            ForgotPasswordSent = "A link is on its way.",
            ResetPasswordTitle = "Choose a new password",
            ResetPasswordSubmit = "Save password",
            ResetPasswordDone = "Your password is changed.",
            ResetPasswordInvalid = "This link has expired or was already used.",
        },
    };

    private static string TokenOf(Uri link) => QueryHelpers.ParseQuery(link.Query)["token"].ToString();
}
