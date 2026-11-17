using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SignInPage.AspNetCore;

/// <summary>Maps the forgot-password and reset pages; chain RequireRateLimiting on the returned group.</summary>
public static class PasswordResetEndpoints
{
    public static RouteGroupBuilder MapSignInPasswordReset(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var paths = app.ServiceProvider.GetRequiredService<PasswordResetPaths>();
        var group = app.MapGroup(string.Empty);
        group.MapGet(paths.Forgot, (PasswordResetFlow flow) => Html(flow.GetForgot()));
        group.MapPost(paths.Forgot, async (HttpContext http, PasswordResetFlow flow, CancellationToken cancellationToken) =>
        {
            var form = await http.Request.ReadFormAsync(cancellationToken);
            return Html(flow.PostForgot(form["email"]));
        });
        group.MapGet(paths.Reset, (string? token, PasswordResetFlow flow) => Html(flow.GetReset(token)));
        group.MapPost(paths.Reset, async (HttpContext http, PasswordResetFlow flow, CancellationToken cancellationToken) =>
        {
            var form = await http.Request.ReadFormAsync(cancellationToken);
            return Html(await flow.PostResetAsync(form["token"], form["password"], cancellationToken));
        });
        return group;
    }

    private static IResult Html(string html) => Results.Content(html, "text/html; charset=utf-8");
}
