namespace SignInPage.AspNetCore;

/// <summary>Server re-check of the field rules the page checks inline; keys are username and password.</summary>
public static class SignInFieldValidator
{
    public const string UsernameField = "username";

    public const string PasswordField = "password";

    public static IReadOnlyDictionary<string, string> Validate(SignInConfig config, string? username, string? password)
    {
        ArgumentNullException.ThrowIfNull(config);
        var errors = new Dictionary<string, string>(StringComparer.Ordinal);
        if (FieldCheck.Message(FieldCheck.UsernameRule(config.Validation.Username), config.Copy, config.Copy.UsernameLabel, username) is { } usernameError)
            errors[UsernameField] = usernameError;
        if (ValidatePassword(config, password) is { } passwordError)
            errors[PasswordField] = passwordError;
        return errors;
    }

    public static string? ValidatePassword(SignInConfig config, string? password)
    {
        ArgumentNullException.ThrowIfNull(config);
        return FieldCheck.Message(config.Validation.Password, config.Copy, config.Copy.PasswordLabel, password);
    }
}
