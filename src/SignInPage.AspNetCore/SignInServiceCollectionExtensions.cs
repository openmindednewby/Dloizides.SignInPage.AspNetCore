using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SignInPage.AspNetCore;

/// <summary>Registers the validated sign-in config and the password reset flow.</summary>
public static class SignInServiceCollectionExtensions
{
    public static IServiceCollection AddSignInConfig(this IServiceCollection services, string path, SignInConfigParseOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(SignInConfigParser.Load(path, options));
        return services;
    }

    public static IServiceCollection AddSignInPasswordReset(this IServiceCollection services, SignInPageShell shell, PasswordResetPaths paths)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(paths);
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton(shell);
        services.TryAddSingleton(paths);
        services.TryAddSingleton<PasswordResetQueue>();
        services.TryAddScoped<PasswordResetService>();
        services.TryAddScoped<PasswordResetFlow>();
        services.AddHostedService<PasswordResetWorker>();
        return services;
    }
}
