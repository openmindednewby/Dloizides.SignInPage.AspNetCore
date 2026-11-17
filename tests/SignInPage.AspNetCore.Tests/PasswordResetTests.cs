using Microsoft.Extensions.Time.Testing;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class PasswordResetTests
{
    [Fact]
    public async Task ResetAsync_WithFreshToken_SetsThePassword()
    {
        var (host, service) = await IssuedFor("ana@acme.test", "u1", new FakeTimeProvider());

        var result = await service.ResetAsync(PasswordResetSamples.TokenOf(host.SentLinks[0]), "newpass123", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.Done, result.Status);
        Assert.Equal("newpass123", host.Passwords["u1"]);
    }

    [Fact]
    public async Task ResetAsync_WithUsedToken_ReturnsInvalidToken()
    {
        var (host, service) = await IssuedFor("ana@acme.test", "u1", new FakeTimeProvider());
        var token = PasswordResetSamples.TokenOf(host.SentLinks[0]);
        await service.ResetAsync(token, "newpass123", CancellationToken.None);

        var second = await service.ResetAsync(token, "otherpass456", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.InvalidToken, second.Status);
        Assert.Equal("newpass123", host.Passwords["u1"]);
    }

    [Fact]
    public async Task ResetAsync_WhenTokenExpired_ReturnsInvalidToken()
    {
        var time = new FakeTimeProvider();
        var (host, service) = await IssuedFor("ana@acme.test", "u1", time);
        time.Advance(TimeSpan.FromMinutes(31));

        var result = await service.ResetAsync(PasswordResetSamples.TokenOf(host.SentLinks[0]), "newpass123", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.InvalidToken, result.Status);
        Assert.Empty(host.Passwords);
    }

    [Fact]
    public async Task ResetAsync_WithPasswordFailingRules_KeepsTheTokenUsable()
    {
        var (host, service) = await IssuedFor("ana@acme.test", "u1", new FakeTimeProvider());
        var token = PasswordResetSamples.TokenOf(host.SentLinks[0]);

        var rejected = await service.ResetAsync(token, "short", CancellationToken.None);

        Assert.Equal(PasswordResetStatus.InvalidPassword, rejected.Status);
        Assert.Equal("Password needs at least 8 characters", rejected.FieldErrors["password"]);
        Assert.Equal(PasswordResetStatus.Done, (await service.ResetAsync(token, "longenough1", CancellationToken.None)).Status);
    }

    private static async Task<(FakeResetHost Host, PasswordResetService Service)> IssuedFor(string email, string userId, TimeProvider time)
    {
        var host = new FakeResetHost { Users = { [email] = userId } };
        var config = PasswordResetSamples.Config(30, 8) with
        {
            Copy = PasswordResetSamples.Config(30, 8).Copy with { PasswordLabel = "Password", TooShortMessage = "{field} needs at least {min} characters" },
        };
        var service = PasswordResetSamples.Service(host, config, time, new PasswordResetQueue());
        await service.ProcessAsync(email, CancellationToken.None);
        return (host, service);
    }
}
