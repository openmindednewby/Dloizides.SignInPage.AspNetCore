using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class PasswordResetRequestTests
{
    [Fact]
    public void Request_WithKnownEmail_QueuesWithoutTouchingTheStore()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var queue = new PasswordResetQueue();
        var service = PasswordResetSamples.Service(host, PasswordResetSamples.Config(30, 8), new FakeTimeProvider(), queue);

        service.Request("ana@acme.test");

        Assert.Equal(0, host.Lookups);
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Request_WithUnknownEmail_QueuesTheSameWay()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var queue = new PasswordResetQueue();
        var service = PasswordResetSamples.Service(host, PasswordResetSamples.Config(30, 8), new FakeTimeProvider(), queue);

        service.Request("nobody@acme.test");

        Assert.Equal(0, host.Lookups);
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public async Task ProcessAsync_WithDifferentlyCasedEmail_MailsOnlyTheStoredAddress()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var service = PasswordResetSamples.Service(host, PasswordResetSamples.Config(30, 8), new FakeTimeProvider(), new PasswordResetQueue());

        await service.ProcessAsync(" ANA@Acme.test ", CancellationToken.None);

        Assert.Equal(["ana@acme.test"], host.SentTo);
    }

    [Fact]
    public async Task ProcessAsync_WithUnknownEmail_SendsNothingAndStoresNothing()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var service = PasswordResetSamples.Service(host, PasswordResetSamples.Config(30, 8), new FakeTimeProvider(), new PasswordResetQueue());

        await service.ProcessAsync("nobody@acme.test", CancellationToken.None);

        Assert.Empty(host.SentTo);
        Assert.Empty(host.Tokens);
    }

    [Fact]
    public async Task ProcessAsync_WithKnownEmail_BuildsTheLinkFromThePublicOrigin()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var service = new PasswordResetService(host, PasswordResetSamples.Config(30, 8), new FakeTimeProvider(), new PasswordResetQueue(),
            PasswordResetSamples.Paths("https://id.acme.test"));

        await service.ProcessAsync("ana@acme.test", CancellationToken.None);

        var link = Assert.Single(host.SentLinks);
        Assert.StartsWith("https://id.acme.test/reset-password?token=", link.AbsoluteUri, StringComparison.Ordinal);
        Assert.DoesNotContain(PasswordResetSamples.TokenOf(link), host.Tokens.Keys);
    }

    [Fact]
    public void PostForgot_WithKnownAndUnknownEmail_AnswersTheSamePage()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var config = PasswordResetSamples.Config(30, 8) with { Copy = PasswordResetSamples.Config(30, 8).Copy with { ForgotPasswordSent = "If an account exists, a link is on its way." } };
        var service = PasswordResetSamples.Service(host, config, new FakeTimeProvider(), new PasswordResetQueue());
        var flow = new PasswordResetFlow(service, config, SignInPageSamples.Shell(), PasswordResetSamples.Paths("https://id.acme.test"));
        var known = flow.PostForgot("ana@acme.test");

        var unknown = flow.PostForgot("nobody@acme.test");

        Assert.Equal(known, unknown);
        Assert.Contains("If an account exists, a link is on its way.", known);
    }

    [Fact]
    public async Task Worker_WithQueuedEmail_SendsTheLinkToTheStoredAddress()
    {
        var host = new FakeResetHost { Users = { ["ana@acme.test"] = "u1" } };
        var services = new ServiceCollection().AddLogging().AddSingleton(PasswordResetSamples.Config(30, 8)).AddSingleton<IPasswordResetHost>(host);
        services.AddSignInPasswordReset(SignInPageSamples.Shell(), PasswordResetSamples.Paths("https://id.acme.test"));
        await using var provider = services.BuildServiceProvider();
        var worker = provider.GetServices<IHostedService>().OfType<PasswordResetWorker>().Single();
        await worker.StartAsync(CancellationToken.None);

        provider.GetRequiredService<PasswordResetQueue>().Enqueue("ANA@acme.test");

        for (var waited = 0; waited < 100 && host.SentTo.Count == 0; waited++)
            await Task.Delay(50);
        await worker.StopAsync(CancellationToken.None);
        Assert.Equal(["ana@acme.test"], host.SentTo);
    }
}
