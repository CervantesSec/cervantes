namespace Cervantes.IFR.Ldap;

public class LdapUser
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public List<string> Groups { get; set; } = new List<string>();
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
}