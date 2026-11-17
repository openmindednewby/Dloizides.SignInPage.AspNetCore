using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SignInPage.AspNetCore;

/// <summary>Drains the reset queue in the background: lookup, token and mail happen off the request path.</summary>
public sealed partial class PasswordResetWorker : BackgroundService
{
    private readonly PasswordResetQueue _queue;

    private readonly IServiceScopeFactory _scopes;

    private readonly ILogger<PasswordResetWorker> _logger;

    public PasswordResetWorker(PasswordResetQueue queue, IServiceScopeFactory scopes, ILogger<PasswordResetWorker> logger)
    {
        _queue = queue;
        _scopes = scopes;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var email in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = _scopes.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<PasswordResetService>().ProcessAsync(email, stoppingToken);
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                LogFailed(_logger, error);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Password reset request failed")]
    private static partial void LogFailed(ILogger logger, Exception error);
}
