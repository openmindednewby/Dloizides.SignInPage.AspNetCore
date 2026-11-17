using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace SignInPage.AspNetCore;

/// <summary>Queues reset requests, issues single-use expiring links to the stored address, applies new passwords.</summary>
public sealed class PasswordResetService
{
    private const int DefaultTokenMinutes = 30;

    private const int TokenBytes = 32;

    private const int MaxEmailLength = 320;

    private static readonly IReadOnlyDictionary<string, string> NoErrors = new Dictionary<string, string>();

    private readonly IPasswordResetHost _host;

    private readonly SignInConfig _config;

    private readonly TimeProvider _time;

    private readonly PasswordResetQueue _queue;

    private readonly PasswordResetPaths _paths;

    public PasswordResetService(IPasswordResetHost host, SignInConfig config, TimeProvider time, PasswordResetQueue queue, PasswordResetPaths paths)
    {
        _host = host;
        _config = config;
        _time = time;
        _queue = queue;
        _paths = paths;
    }

    public void Request(string? email)
    {
        var address = email?.Trim();
        if (string.IsNullOrEmpty(address) || address.Length > MaxEmailLength)
            return;
        _queue.Enqueue(address);
    }

    public async Task ProcessAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _host.FindUserAsync(email.Trim(), cancellationToken);
        if (user is null)
            return;

        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenBytes));
        var minutes = _config.Features.ForgotPassword?.TokenMinutes ?? DefaultTokenMinutes;
        await _host.SaveResetTokenAsync(new PasswordResetToken(Hash(token), user.UserId, _time.GetUtcNow().AddMinutes(minutes)), cancellationToken);
        var link = new UriBuilder(new Uri(_paths.PublicOrigin, _paths.Reset)) { Query = $"token={token}" }.Uri;
        await _host.SendResetLinkAsync(user, link, cancellationToken);
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
