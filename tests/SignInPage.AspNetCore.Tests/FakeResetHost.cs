using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

internal sealed class FakeResetHost : IPasswordResetHost
{
    public Dictionary<string, string> Users { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, PasswordResetToken> Tokens { get; } = [];

    public List<Uri> SentLinks { get; } = [];

    public Dictionary<string, string> Passwords { get; } = [];

    public Task<string?> FindUserIdAsync(string email, CancellationToken cancellationToken) =>
        Task.FromResult(Users.TryGetValue(email, out var id) ? id : null);

    public Task SaveResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken)
    {
        Tokens[token.TokenHash] = token;
        return Task.CompletedTask;
    }

    public Task<PasswordResetToken?> TakeResetTokenAsync(string tokenHash, CancellationToken cancellationToken) =>
        Task.FromResult(Tokens.Remove(tokenHash, out var token) ? token : null);

    public Task SendResetLinkAsync(string userId, string email, Uri resetLink, CancellationToken cancellationToken)
    {
        SentLinks.Add(resetLink);
        return Task.CompletedTask;
    }

    public Task SetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken)
    {
        Passwords[userId] = newPassword;
        return Task.CompletedTask;
    }
}
