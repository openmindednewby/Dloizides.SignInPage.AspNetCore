namespace SignInPage.AspNetCore;

/// <summary>The forgot-password and reset pages as HTML: what each GET and POST answers.</summary>
public sealed class PasswordResetFlow
{
    private readonly PasswordResetService _service;

    private readonly SignInConfig _config;

    private readonly SignInPageShell _shell;

    private readonly PasswordResetPaths _paths;

    public PasswordResetFlow(PasswordResetService service, SignInConfig config, SignInPageShell shell, PasswordResetPaths paths)
    {
        _service = service;
        _config = config;
        _shell = shell;
        _paths = paths;
    }

    public string GetForgot() => PasswordResetPages.Request(_shell, _config, _paths);

    public string PostForgot(string? email)
    {
        _service.Request(email);
        return PasswordResetPages.Sent(_shell, _config, _paths);
    }

    public string GetReset(string? token) =>
        string.IsNullOrEmpty(token)
            ? PasswordResetPages.Invalid(_shell, _config, _paths)
            : PasswordResetPages.Reset(_shell, _config, _paths, token, null);

    public async Task<string> PostResetAsync(string? token, string? newPassword, CancellationToken cancellationToken)
    {
        var result = await _service.ResetAsync(token, newPassword, cancellationToken);
        return result.Status switch
        {
            PasswordResetStatus.Done => PasswordResetPages.Done(_shell, _config, _paths),
            PasswordResetStatus.InvalidPassword => PasswordResetPages.Reset(_shell, _config, _paths, token ?? string.Empty,
                result.FieldErrors.GetValueOrDefault(SignInFieldValidator.PasswordField)),
            _ => PasswordResetPages.Invalid(_shell, _config, _paths),
        };
    }
}
