namespace SignInPage.AspNetCore;

/// <summary>Host user store, token store and mailer; TakeResetTokenAsync deletes the token it returns.</summary>
public interface IPasswordResetHost
{
    Task<PasswordResetUser?> FindUserAsync(string email, CancellationToken cancellationToken);

    Task SaveResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken);

    Task<PasswordResetToken?> TakeResetTokenAsync(string tokenHash, CancellationToken cancellationToken);

    Task SendResetLinkAsync(PasswordResetUser user, Uri resetLink, CancellationToken cancellationToken);

    Task SetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken);
}

/// <summary>A user found by email: its id and the email address stored for it, the only address mailed.</summary>
public sealed record PasswordResetUser(string UserId, string Email);

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

/// <summary>The configured public origin reset links are built from, and where the three pages live.</summary>
public sealed record PasswordResetPaths
{
    public required Uri PublicOrigin { get; init; }

    public string SignIn { get; init; } = "/login";

    public string Forgot { get; init; } = "/forgot-password";

    public string Reset { get; init; } = "/reset-password";
}
