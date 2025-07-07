using Microsoft.AspNetCore.Authentication;

namespace Cervantes.Web.Authentication;

public class LdapAuthenticationOptions : AuthenticationSchemeOptions
{
    public string CallbackPath { get; set; } = "/Account/ExternalLogin";
    public string SignInScheme { get; set; } = "External";
}