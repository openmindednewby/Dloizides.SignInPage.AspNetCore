namespace SignInPage.AspNetCore;

/// <summary>Host limits checked at load; SessionMaxHours is the app's SESSION-1 maximum session length.</summary>
public sealed record SignInConfigParseOptions(double? SessionMaxHours = null);

/// <summary>One config problem: a JSON Pointer to the field, the failed rule and a readable message.</summary>
public sealed record SignInConfigError(string Path, string Keyword, string Message);

/// <summary>The parsed config when valid, otherwise every error sorted by path then keyword.</summary>
public sealed record SignInConfigResult(SignInConfig? Config, IReadOnlyList<SignInConfigError> Errors);

/// <summary>Thrown at startup when sign-in.json is invalid; the message names every field path.</summary>
public sealed class InvalidSignInConfigException : Exception
{
    public InvalidSignInConfigException(string source, IReadOnlyList<SignInConfigError> errors)
        : base($"Invalid sign-in config {source}:\n{string.Join("\n", errors.Select(error => $"{(error.Path.Length == 0 ? "/" : error.Path)}: {error.Message}"))}")
    {
        Errors = errors;
    }

    public IReadOnlyList<SignInConfigError> Errors { get; }
}
