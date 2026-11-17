namespace SignInPage.AspNetCore;

/// <summary>Host user store, token store and mailer; TakeResetTokenAsync deletes the token it returns.</summary>
public interface IPasswordResetHost
{
    Task<string?> FindUserIdAsync(string email, CancellationToken cancellationToken);

    Task SaveResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken);

    Task<PasswordResetToken?> TakeResetTokenAsync(string tokenHash, CancellationToken cancellationToken);

    Task SendResetLinkAsync(string userId, string email, Uri resetLink, CancellationToken cancellationToken);

    Task SetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken);
}

/// <summary>A stored reset token: only the SHA-256 hex of the emailed token, its user and its expiry.</summary>
public sealed record PasswordResetToken(string TokenHash, string UserId, DateTimeOffset ExpiresAt);

/// <summary>The outcome of a reset attempt.</summary>
public enum PasswordResetStatus
{
    Done = 0,
    InvalidToken = 1,
    InvalidPassword = 2,
}

/// <summary>A reset outcome plus the field errors when the new password fails the rules.</summary>
public sealed record PasswordResetResult(PasswordResetStatus Status, IReadOnlyDictionary<string, string> FieldErrors);

/// <summary>Where the sign-in, forgot-password and reset pages live.</summary>
public sealed record PasswordResetPaths
{
    public string SignIn { get; init; } = "/login";

    public string Forgot { get; init; } = "/forgot-password";

    public string Reset { get; init; } = "/reset-password";
}
