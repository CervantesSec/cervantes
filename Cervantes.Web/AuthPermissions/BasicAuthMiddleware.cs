using AuthPermissions.AspNetCore.Services;
using AuthPermissions.BaseCode.CommonCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthPermissions;
using AuthPermissions.AdminCode;
using AuthPermissions.BaseCode.DataLayer.Classes;
using AuthPermissions.BaseCode.PermissionsCode;
using Cervantes.CORE.Entities;
using Task = System.Threading.Tasks.Task;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BasicAuthMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BasicAuthMiddleware(
        RequestDelegate next,
        ILogger<BasicAuthMiddleware> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

  public async Task InvokeAsync(HttpContext context)
{
    try
    {
        // 1. Filtrar solo rutas API
        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // 2. Evitar procesamiento si ya está autenticado
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        // 3. Validar cabecera de autenticación
        if (!TryGetBasicCredentials(context, out var username, out var password))
        {
            await ChallengeAsync(context);
            return;
        }

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            // 4. Buscar usuario en Identity
            var user = await FindIdentityUser(scope, username);
            if (user == null)
            {
                _logger.LogWarning($"User not found: {username}");
                await ChallengeAsync(context);
                return;
            }

            // 5. Validar contraseña
            if (!await ValidatePassword(scope, user, password))
            {
                _logger.LogWarning($"Invalid password for user: {username}");
                await ChallengeAsync(context);
                return;
            }

            // 6. Obtener claims combinados
            var claims = await BuildUserClaims(scope, user);
            
            // 7. Crear identidad autenticada
            context.User = CreateClaimsPrincipal(claims, "BasicAuth");
            
            _logger.LogInformation($"User {username} authenticated successfully with {claims.Count} claims");
        }

        await _next(context);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in BasicAuthMiddleware");
        await HandleAuthError(context);
    }
}

// Métodos auxiliares
private bool TryGetBasicCredentials(HttpContext context, out string username, out string password)
{
    username = password = null;
    
    if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        return false;

    var authString = authHeader.ToString();
    if (!authString.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        return false;

    try
    {
        var encodedCredentials = authString["Basic ".Length..].Trim();
        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var credentials = decodedCredentials.Split(':', 2);
        
        if (credentials.Length != 2) return false;
        
        username = credentials[0];
        password = credentials[1];
        return true;
    }
    catch
    {
        return false;
    }
}

private async Task<ApplicationUser> FindIdentityUser(IServiceScope scope, string username)
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    return await userManager.FindByNameAsync(username) 
           ?? await userManager.FindByEmailAsync(username);
}

private async Task<bool> ValidatePassword(IServiceScope scope, ApplicationUser user, string password)
{
    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
    return (await signInManager.CheckPasswordSignInAsync(user, password, false)).Succeeded;
}

private async Task<List<Claim>> BuildUserClaims(IServiceScope scope, ApplicationUser user)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Email, user.Email),
        new("Permission", "ApiAccess")
    };

    // Claims desde Identity Roles
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    foreach (var role in await userManager.GetRolesAsync(user))
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    // Claims desde AuthPermissions
    await AddAuthPermissionClaims(scope, user, claims);
    
    return claims;
}

private async Task AddAuthPermissionClaims(IServiceScope scope, ApplicationUser user, List<Claim> claims)
{
    try
    {
        var authUsersAdmin = scope.ServiceProvider.GetRequiredService<IAuthUsersAdminService>();
        var claimsCalculator = scope.ServiceProvider.GetRequiredService<IClaimsCalculator>();

        // 1. Buscar usuario en AuthPermissions usando el UserId de Identity
        var authUser = await authUsersAdmin.FindAuthUserByUserIdAsync(user.Id);
        if (authUser == null)
        {
            _logger.LogWarning($"AuthUser not found for Identity user: {user.Id}");
            return;
        }

        // 2. Obtener claims específicos de AuthPermissions
        var authClaims = await claimsCalculator.GetClaimsForAuthUserAsync(authUser.Result.UserId);
        if (authClaims != null)
        {
            claims.AddRange(authClaims);
            _logger.LogInformation($"Added {authClaims.Count} AuthPermission claims");
        }

        // 3. Asegurar DataKey claim
        await EnsureDataKeyClaim(authUser.Result, claims);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding AuthPermission claims");
    }
}

private async Task EnsureDataKeyClaim(AuthUser authUser, List<Claim> claims)
{
    var existingDataKey = claims.FirstOrDefault(c => c.Type == PermissionConstants.DataKeyClaimType);
    if (existingDataKey != null) return;

    // Obtener DataKey del tenant si existe
    var dataKey = authUser.UserTenant?.GetTenantDataKey() ?? await GetDefaultDataKey();
    claims.Add(new Claim(PermissionConstants.DataKeyClaimType, dataKey));
    
    _logger.LogInformation($"Added DataKey claim: {dataKey}");
}

private async Task<string> GetDefaultDataKey()
{
    // Lógica para obtener DataKey por defecto si es necesario
    return "1"; // Ejemplo básico
}

private ClaimsPrincipal CreateClaimsPrincipal(IEnumerable<Claim> claims, string authScheme)
{
    var identity = new ClaimsIdentity(claims, authScheme);
    return new ClaimsPrincipal(identity);
}

private async Task HandleAuthError(HttpContext context)
{
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    await context.Response.WriteAsync("Authentication error");
}

    private async Task ChallengeAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"API Cervantes\"");
        await context.Response.WriteAsync("Authentication required for API access.");
    }
}

public static class BasicAuthExtensions
{

    public static IApplicationBuilder UseBasicAuthForApi(this IApplicationBuilder app, string pathPrefix = "/api")
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        return app.UseWhen(
            context => context.Request.Path.StartsWithSegments(pathPrefix, StringComparison.OrdinalIgnoreCase),
            appBuilder => appBuilder.UseMiddleware<BasicAuthMiddleware>()
        );
    }


    public static IApplicationBuilder UseBasicAuthWhen(
        this IApplicationBuilder app, 
        Func<HttpContext, bool> predicate)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));
        
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return app.UseWhen(predicate, appBuilder => appBuilder.UseMiddleware<BasicAuthMiddleware>());
    }


    public static IApplicationBuilder UseBasicAuthForPaths(
        this IApplicationBuilder app, 
        params string[] pathPrefixes)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));
        
        if (pathPrefixes == null || pathPrefixes.Length == 0)
            throw new ArgumentException("Debe proporcionar al menos un prefijo de ruta", nameof(pathPrefixes));

        return app.UseWhen(context => 
        {
            foreach (var prefix in pathPrefixes)
            {
                if (context.Request.Path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }, appBuilder => appBuilder.UseMiddleware<BasicAuthMiddleware>());
    }
}