using System.Security.Claims;
using System.Text.Json;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Cervantes.Web.Components.Account.Pages;
using Cervantes.Web.Components.Account.Pages.Manage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Task = System.Threading.Tasks.Task;
using Cervantes.Web.Components.Account;
using Microsoft.AspNetCore.Components;
using Cervantes.IFR.Ldap;
using AuthPermissions.AdminCode;


namespace Microsoft.AspNetCore.Routing;


internal static class IdentityComponentsEndpointRouteBuilderExtensions
{

    public const string LoginCallbackAction = "LoginCallback";

    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/PerformExternalLogin", (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("ReturnUrl", returnUrl),
                new("Action", LoginCallbackAction)];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/ExternalLogin",
                QueryString.Create(query));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        accountGroup.MapPost("/PerformLdapLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] ILdapService ldapService,
            [FromServices] ILdapConfiguration ldapConfiguration,
            [FromServices] IAuthUsersAdminService authUsersAdminService,
            [FromServices] ILogger<Program> logger,
            [FromForm] string username,
            [FromForm] string password) =>
        {
            try
            {
                // Validate LDAP credentials
                var isValid = await ldapService.ValidateUserAsync(username, password);
                if (!isValid)
                {
                    return TypedResults.LocalRedirect("~/Account/LdapLogin?errorMessage=Invalid%20LDAP%20credentials");
                }

                // Get user information from LDAP
                var ldapUser = await ldapService.GetUserInfoAsync(username);
                if (ldapUser == null)
                {
                    return TypedResults.LocalRedirect("~/Account/LdapLogin?errorMessage=Could%20not%20retrieve%20user%20information%20from%20LDAP");
                }

                // Try to find existing user
                var existingUser = await userManager.FindByNameAsync(username);
                if (existingUser != null)
                {
                    // Mark as external and assign roles if needed
                    if (!existingUser.ExternalLogin)
                    {
                        existingUser.ExternalLogin = true;
                        await userManager.UpdateAsync(existingUser);
                    }

                    var authUser = await authUsersAdminService.FindAuthUserByUserIdAsync(existingUser.Id);
                    if (authUser.Result == null || authUser.Result.UserRoles.Count == 0)
                    {
                        var roleToAssign = DetermineUserRole(ldapUser.Groups, ldapConfiguration);
                        var roles = new List<string> { roleToAssign };
                        await authUsersAdminService.AddNewUserAsync(existingUser.Id, existingUser.Email, existingUser.UserName, roles);
                    }

                    await signInManager.SignInAsync(existingUser, isPersistent: true);
                    
                    // Always redirect to home page to avoid redirection issues
                    logger.LogInformation("LDAP Login successful for user {Username}, redirecting to home", username);
                    return TypedResults.LocalRedirect("~/");
                }

                // Create new user
                if (string.IsNullOrEmpty(ldapUser.Email))
                {
                    return TypedResults.LocalRedirect("~/Account/LdapLogin?errorMessage=No%20email%20address%20found%20in%20LDAP");
                }

                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = ldapUser.Email,
                    FullName = ldapUser.DisplayName ?? username,
                    EmailConfirmed = true,
                    ExternalLogin = true
                };

                var createResult = await userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    var roleToAssign = DetermineUserRole(ldapUser.Groups, ldapConfiguration);
                    var roles = new List<string> { roleToAssign };
                    await authUsersAdminService.AddNewUserAsync(user.Id, user.Email, user.UserName, roles);

                    await signInManager.SignInAsync(user, isPersistent: true);
                    
                    // Always redirect to home page to avoid redirection issues
                    logger.LogInformation("LDAP Login successful for new user {Username}, redirecting to home", username);
                    return TypedResults.LocalRedirect("~/");
                }

                return TypedResults.LocalRedirect("~/Account/LdapLogin?errorMessage=Error%20creating%20user%20account");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during LDAP login for user {Username}", username);
                return TypedResults.LocalRedirect("~/Account/LdapLogin?errorMessage=An%20error%20occurred%20during%20authentication");
            }
        });

        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        /*manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider) =>
        {
            // Clear the existing external cookie to ensure a clean login process
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/Manage/ExternalLogins",
                QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl,
                signInManager.UserManager.GetUserId(context.User));
            return TypedResults.Challenge(properties, [provider]);
        });*/

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            }

            var userId = await userManager.GetUserIdAsync(user);
            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
        });

        return accountGroup;
    }

    /// <summary>
    /// Maps LDAP groups to AuthPermissions roles using configuration
    /// </summary>
    /// <param name="ldapGroups">List of LDAP groups the user belongs to</param>
    /// <param name="ldapConfiguration">LDAP configuration with role mapping</param>
    /// <returns>The role to assign, defaults to configured DefaultRole</returns>
    private static string DetermineUserRole(IList<string>? ldapGroups, ILdapConfiguration ldapConfiguration)
    {
        if (ldapGroups == null || !ldapGroups.Any())
        {
            return ldapConfiguration.DefaultRole; // Default role from configuration
        }

        // Use configured group to role mapping
        var groupRoleMapping = ldapConfiguration.GroupRoleMapping ?? new Dictionary<string, string>();

        // Check if user belongs to any configured groups (case-insensitive)
        foreach (var group in ldapGroups)
        {
            var mappedRole = groupRoleMapping.FirstOrDefault(kvp => 
                string.Equals(kvp.Key, group, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrEmpty(mappedRole.Value))
            {
                return mappedRole.Value;
            }
        }

        // Default to configured default role
        return ldapConfiguration.DefaultRole;
    }
}