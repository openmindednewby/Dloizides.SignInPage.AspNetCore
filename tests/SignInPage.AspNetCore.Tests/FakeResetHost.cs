using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

internal sealed class FakeResetHost : IPasswordResetHost
{
    public Dictionary<string, string> Users { get; } = [];

    public Dictionary<string, PasswordResetToken> Tokens { get; } = [];

    public List<Uri> SentLinks { get; } = [];

    public List<string> SentTo { get; } = [];

    public Dictionary<string, string> Passwords { get; } = [];

    public int Lookups { get; private set; }

    public Task<PasswordResetUser?> FindUserAsync(string email, CancellationToken cancellationToken)
    {
        Lookups++;
        var stored = Users.Keys.FirstOrDefault(key => string.Equals(key, email.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(stored is null ? null : new PasswordResetUser(Users[stored], stored));
    }

    public Task SaveResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken)
    {
        Tokens[token.TokenHash] = token;
        return Task.CompletedTask;
    }

    public Task<PasswordResetToken?> TakeResetTokenAsync(string tokenHash, CancellationToken cancellationToken) =>
        Task.FromResult(Tokens.Remove(tokenHash, out var token) ? token : null);

    public Task SendResetLinkAsync(PasswordResetUser user, Uri resetLink, CancellationToken cancellationToken)
    {
        lock (SentLinks)
        {
            SentLinks.Add(resetLink);
            SentTo.Add(user.Email);
        }

        return Task.CompletedTask;
    }

    public Task SetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken)
    {
        Passwords[userId] = newPassword;
        return Task.CompletedTask;
    }
}
