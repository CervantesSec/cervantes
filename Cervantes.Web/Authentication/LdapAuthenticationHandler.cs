using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Cervantes.IFR.Ldap;
using Cervantes.CORE.Entities;

namespace Cervantes.Web.Authentication;

public class LdapAuthenticationHandler : AuthenticationHandler<LdapAuthenticationOptions>
{
    private readonly ILdapService _ldapService;

    public LdapAuthenticationHandler(
        IOptionsMonitor<LdapAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ILdapService ldapService)
        : base(options, logger, encoder)
    {
        _ldapService = ldapService;
    }

    protected override async System.Threading.Tasks.Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // This handler is designed to work with external login flow
        // The actual authentication is handled by the challenge method
        return AuthenticateResult.NoResult();
    }

    protected override async System.Threading.Tasks.Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // For LDAP, we redirect to a login page where users enter credentials
        // This is different from OAuth flows that redirect to external providers
        
        var redirectUri = properties.RedirectUri ?? "/";
        var callbackUrl = BuildRedirectUri(Options.CallbackPath);
        
        // Store the original redirect URI in the state
        properties.Items["returnUrl"] = redirectUri;
        
        // Redirect to LDAP login page with callback information
        var loginUrl = $"/Account/LdapLogin?returnUrl={Uri.EscapeDataString(redirectUri)}&scheme={LdapDefaults.AuthenticationScheme}";
        
        Response.Redirect(loginUrl);
    }

    public async System.Threading.Tasks.Task<AuthenticateResult> AuthenticateWithCredentialsAsync(string username, string password)
    {
        if (!_ldapService.LdapEnabled())
        {
            return AuthenticateResult.Fail("LDAP authentication is not enabled");
        }

        try
        {
            // Validate user credentials against LDAP
            var isValid = await _ldapService.ValidateUserAsync(username, password);
            if (!isValid)
            {
                return AuthenticateResult.Fail("Invalid LDAP credentials");
            }

            // Get user information from LDAP
            Logger.LogInformation("Retrieving user info for {Username}", username);
            var ldapUser = await _ldapService.GetUserInfoAsync(username);
            if (ldapUser == null)
            {
                Logger.LogWarning("Could not retrieve user information from LDAP for {Username}", username);
                return AuthenticateResult.Fail("Could not retrieve user information from LDAP");
            }
            
            Logger.LogInformation("Successfully retrieved LDAP user info for {Username}, Email: {Email}", username, ldapUser.Email);

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, ldapUser.Username),
                new Claim(ClaimTypes.Name, ldapUser.DisplayName ?? ldapUser.Username),
                new Claim(ClaimTypes.Email, ldapUser.Email),
                new Claim(ClaimTypes.GivenName, ldapUser.FirstName),
                new Claim(ClaimTypes.Surname, ldapUser.LastName),
                new Claim("ldap_dn", ldapUser.DistinguishedName)
            };

            // Add group claims
            if (ldapUser.Groups != null)
            {
                foreach (var group in ldapUser.Groups)
                {
                    claims.Add(new Claim(ClaimTypes.Role, group));
                    claims.Add(new Claim("ldap_group", group));
                }
            }

            // Add additional LDAP attributes as claims
            if (ldapUser.Attributes != null)
            {
                foreach (var attribute in ldapUser.Attributes)
                {
                    claims.Add(new Claim($"ldap_{attribute.Key}", attribute.Value));
                }
            }

            var identity = new ClaimsIdentity(claims, LdapDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, LdapDefaults.AuthenticationScheme);
            
            Logger.LogInformation("LDAP authentication successful for user {Username}", username);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during LDAP authentication for user {Username}", username);
            return AuthenticateResult.Fail($"LDAP authentication error: {ex.Message}");
        }
    }
}