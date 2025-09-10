using System.Net;
using System.Security.Claims;
using AuthPermissions;
using AuthPermissions.AspNetCore.Services;
using AuthPermissions.AdminCode;
using AuthPermissions.BaseCode.CommonCode;
using Cervantes.Contracts;
using Cervantes.CORE.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cervantes.Web.AuthPermissions;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly ApiKeyOptions _options;

    public ApiKeyMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyMiddleware> logger,
        IOptions<ApiKeyOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Sólo aplicar a rutas /api
        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Si ya hay user autenticado, seguir
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        var rawKey = ReadKeyFromHeaders(context);
        if (string.IsNullOrEmpty(rawKey))
        {
            await _next(context); // No header => permitir BasicAuth u otros
            return;
        }

        if (_options.RequireHttps && context.Request.IsHttps == false)
        {
            await WriteUnauthorized(context, "API Key requires HTTPS");
            return;
        }

        // Resolve scoped services from the current request
        var apiKeyManager = context.RequestServices.GetRequiredService<IApiKeyManager>();
        var userManager = context.RequestServices.GetRequiredService<UserManager<Cervantes.CORE.Entities.ApplicationUser>>();
        var authUsersAdmin = context.RequestServices.GetRequiredService<IAuthUsersAdminService>();
        var claimsCalculator = context.RequestServices.GetRequiredService<IClaimsCalculator>();

        var result = await apiKeyManager.ValidateAsync(rawKey);
        if (!result.Success || string.IsNullOrEmpty(result.UserId))
        {
            await WriteUnauthorized(context, result.FailureReason ?? "Invalid API Key");
            return;
        }

        var user = await userManager.FindByIdAsync(result.UserId);
        if (user == null)
        {
            await WriteUnauthorized(context, "User not found for API Key");
            return;
        }

        // Construir claims: Identity + AuthPermissions
        var identityClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id),
            new(ClaimTypes.AuthenticationMethod, "ApiKey")
        };

        var authUser = await authUsersAdmin.FindAuthUserByUserIdAsync(user.Id);
        if (authUser?.Result != null)
        {
            var authClaims = await claimsCalculator.GetClaimsForAuthUserAsync(authUser.Result.UserId);
            if (authClaims != null)
                identityClaims.AddRange(authClaims);
        }

        context.User = new ClaimsPrincipal(new ClaimsIdentity(identityClaims, "ApiKey"));
        await _next(context);
    }

    private string? ReadKeyFromHeaders(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(_options.HeaderName, out var value))
            return value.ToString();

        if (context.Request.Headers.TryGetValue("Authorization", out var auth))
        {
            var s = auth.ToString();
            if (s.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
                return s.Substring("ApiKey ".Length).Trim();
        }
        return null;
    }

    private async Task WriteUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await context.Response.WriteAsync(message);
    }
}

public static class ApiKeyAuthExtensions
{
    public static IApplicationBuilder UseApiKeyAuthForApi(this IApplicationBuilder app, string pathPrefix = "/api")
    {
        if (app == null) throw new ArgumentNullException(nameof(app));
        return app.UseWhen(
            context => context.Request.Path.StartsWithSegments(pathPrefix, StringComparison.OrdinalIgnoreCase),
            builder => builder.UseMiddleware<ApiKeyMiddleware>());
    }
}
