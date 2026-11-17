using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace SignInPage.AspNetCore;

/// <summary>Issues single-use expiring reset links; unknown emails get the same answer.</summary>
public sealed class PasswordResetService
{
    private const int DefaultTokenMinutes = 30;

    private const int TokenBytes = 32;

    private static readonly IReadOnlyDictionary<string, string> NoErrors = new Dictionary<string, string>();

    private readonly IPasswordResetHost _host;

    private readonly SignInConfig _config;

    private readonly TimeProvider _time;

    public PasswordResetService(IPasswordResetHost host, SignInConfig config, TimeProvider time)
    {
        _host = host;
        _config = config;
        _time = time;
    }

    public async Task RequestAsync(string? email, Uri resetPage, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(resetPage);
        var address = email?.Trim();
        if (string.IsNullOrEmpty(address))
            return;
        var userId = await _host.FindUserIdAsync(address, cancellationToken);
        if (userId is null)
            return;

        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenBytes));
        var minutes = _config.Features.ForgotPassword?.TokenMinutes ?? DefaultTokenMinutes;
        await _host.SaveResetTokenAsync(new PasswordResetToken(Hash(token), userId, _time.GetUtcNow().AddMinutes(minutes)), cancellationToken);
        var link = new UriBuilder(resetPage) { Query = $"token={token}" }.Uri;
        await _host.SendResetLinkAsync(userId, address, link, cancellationToken);
    }

    public async Task<PasswordResetResult> ResetAsync(string? token, string? newPassword, CancellationToken cancellationToken)
    {
        if (SignInFieldValidator.ValidatePassword(_config, newPassword) is { } error)
            return new PasswordResetResult(PasswordResetStatus.InvalidPassword, new Dictionary<string, string> { [SignInFieldValidator.PasswordField] = error });
        if (string.IsNullOrEmpty(token))
            return new PasswordResetResult(PasswordResetStatus.InvalidToken, NoErrors);

        var stored = await _host.TakeResetTokenAsync(Hash(token), cancellationToken);
        if (stored is null || stored.ExpiresAt <= _time.GetUtcNow())
            return new PasswordResetResult(PasswordResetStatus.InvalidToken, NoErrors);

        await _host.SetPasswordAsync(stored.UserId, newPassword!, cancellationToken);
        return new PasswordResetResult(PasswordResetStatus.Done, NoErrors);
    }

    private static string Hash(string token) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
